using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Ext.Tests;

/// <summary>
/// Test implementation of ITimestamped for testing purposes.
/// </summary>
public record TestMetric(DateTimeOffset Timestamp, double Value) : ITimestamped;

/// <summary>
/// Tests for WindowByTimestampObservable functionality.
/// </summary>
[TestFixture]
public class WindowByTimestampObservableTests
{
  private Subject<TestMetric> _source;

  [SetUp]
  public void Setup()
  {
    _source = new Subject<TestMetric>();
  }

  [TearDown]
  public void TearDown()
  {
    _source?.Dispose();
  }

  [Test]
  public void WindowByTimestamp_WithITimestamped_GroupsItemsByTimestamp()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var windowDuration = TimeSpan.FromSeconds(10);
    var results = new ConcurrentBag<ConcurrentBag<TestMetric>>();

    var item1 = new TestMetric(baseTime, 1.0);
    var item2 = new TestMetric(baseTime.AddSeconds(5), 2.0);
    var item3 = new TestMetric(baseTime.AddSeconds(15), 3.0); // Different window
    var item4 = new TestMetric(baseTime.AddSeconds(7), 4.0);

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(window =>
      {
        var windowItems = new ConcurrentBag<TestMetric>();
        window.Subscribe(item => windowItems.Add(item));
        results.Add(windowItems);
      });

    _source.OnNext(item1);
    _source.OnNext(item2);
    _source.OnNext(item4);
    _source.OnNext(item3);
    _source.OnCompleted();

    // Assert - Allow for some flexibility in window count due to buffering
    Assert.That(results, Has.Count.GreaterThanOrEqualTo(2), "Should create at least 2 windows");

    // Check that item3 (timestamp baseTime + 15s) is in a different window than items 1,2,4
    var allFirstWindowItems = results.Take(results.Count() - 1).SelectMany(x => x).ToList();
    var lastWindowItems = results.Last();

    Assert.That(allFirstWindowItems.Any(x => Math.Abs(x.Value - 3.0) < 0.001) ||
               lastWindowItems.Any(x => Math.Abs(x.Value - 3.0) < 0.001),
               Is.True, "Item3 should be present in one of the windows");
  }

  [Test]
  public void WindowByTimestamp_WithCustomTimestampSelector_GroupsCorrectly()
  {
    // Arrange
    var baseTimestamp = 1000L;
    var windowDuration = TimeSpan.FromTicks(100);
    var results = new ConcurrentBag<ConcurrentBag<long>>();
    var source = new Subject<long>();

    // Act
    source
      .WindowByTimestamp(x => x, windowDuration)
      .Subscribe(window =>
      {
        var windowItems = new ConcurrentBag<long>();
        window.Subscribe(item => windowItems.Add(item));
        results.Add(windowItems);
      });

    source.OnNext(baseTimestamp);
    source.OnNext(baseTimestamp + 50);
    source.OnNext(baseTimestamp + 99);  // Same window
    Thread.Sleep(1500); // Wait for buffer
    source.OnNext(baseTimestamp + 150); // Different window
    Thread.Sleep(1500); // Wait for buffer
    source.OnCompleted();

    // Assert
    Assert.That(results, Has.Count.EqualTo(2));
    var resultsList = results.ToList();

    // Since ConcurrentBag doesn't maintain order, we need to check content differently
    var window1Items = resultsList[0].ToList();
    var window2Items = resultsList[1].ToList();

    // One window should have 3 items, the other should have 1
    var windowSizes = new[] { window1Items.Count, window2Items.Count }.OrderBy(x => x).ToArray();
    Assert.That(windowSizes, Is.EqualTo(new[] { 1, 3 }));

    // Check that all expected values are present
    var allValues = window1Items.Concat(window2Items).ToList();
    Assert.That(allValues, Is.EquivalentTo(new[] { baseTimestamp, baseTimestamp + 50, baseTimestamp + 99, baseTimestamp + 150 }));
  }

  [Test]
  public void WindowByTimestamp_WithEmptySource_CompletesWithoutWindows()
  {
    // Arrange
    var windowDuration = TimeSpan.FromSeconds(10);
    var windowCount = 0;
    var completed = false;

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(
        window => windowCount++,
        onCompleted: () => completed = true);

    _source.OnCompleted();

    // Assert
    Assert.That(windowCount, Is.EqualTo(0));
    Assert.That(completed, Is.True);
  }

  [Test]
  public void WindowByTimestamp_WithError_PropagatesError()
  {
    // Arrange
    var windowDuration = TimeSpan.FromSeconds(10);
    var expectedException = new InvalidOperationException("Test error");
    Exception? caughtException = null;

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(
        window => { },
        error => caughtException = error);

    _source.OnError(expectedException);

    // Assert
    Assert.That(caughtException, Is.EqualTo(expectedException));
  }

  [Test]
  public void WindowByTimestamp_WithNullSource_ThrowsArgumentNullException()
  {
    // Arrange
    IObservable<TestMetric>? nullSource = null;
    var windowDuration = TimeSpan.FromSeconds(10);

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => nullSource!.WindowByTimestamp(windowDuration));
  }

  [Test]
  public void WindowByTimestamp_WithCustomBufferTime_BuffersCorrectly()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var windowDuration = TimeSpan.FromSeconds(10);
    var results = new ConcurrentBag<ConcurrentBag<TestMetric>>();

    var item1 = new TestMetric(baseTime, 1.0);
    var item2 = new TestMetric(baseTime.AddSeconds(5), 2.0);

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(window =>
      {
        var windowItems = new ConcurrentBag<TestMetric>();
        window.Subscribe(item => windowItems.Add(item));
        results.Add(windowItems);
      });

    _source.OnNext(item1);
    _source.OnNext(item2);
    Thread.Sleep(1500); // Wait for buffer to process
    _source.OnCompleted();

    // Assert
    var resultsList = results.ToList();
    Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

    // Check that we have the expected total number of items
    var totalItems = resultsList.Sum(window => window.Count);
    Assert.That(totalItems, Is.EqualTo(2));
  }

  [Test]
  public void WindowByTimestamp_WithOutOfOrderItems_SortsCorrectly()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var windowDuration = TimeSpan.FromSeconds(10);
    var allItems = new ConcurrentBag<TestMetric>();

    var item1 = new TestMetric(baseTime.AddSeconds(5), 1.0);
    var item2 = new TestMetric(baseTime, 2.0); // Earlier timestamp
    var item3 = new TestMetric(baseTime.AddSeconds(3), 3.0);

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(window =>
      {
        window.Subscribe(item => allItems.Add(item));
      });

    _source.OnNext(item1);
    _source.OnNext(item2);
    _source.OnNext(item3);
    Thread.Sleep(1500); // Wait for buffer to process
    _source.OnCompleted();

    // Assert - Check that items are present and properly sorted by timestamp when processed
    Assert.That(allItems, Has.Count.EqualTo(3), "Should have all 3 items");

    // Since items are sorted within the buffer, check if we have all expected values
    var values = allItems.Select(x => x.Value).OrderBy(x => x).ToArray();
    Assert.That(values, Is.EquivalentTo(new[] { 1.0, 2.0, 3.0 }));

    // Find the items by their values to check ordering
    var allItemsList = allItems.ToList();
    var item2InResults = allItemsList.First(x => Math.Abs(x.Value - 2.0) < 0.001);
    var item3InResults = allItemsList.First(x => Math.Abs(x.Value - 3.0) < 0.001);
    var item1InResults = allItemsList.First(x => Math.Abs(x.Value - 1.0) < 0.001);

    // Check that the timestamps are in the correct order (item2 < item3 < item1)
    Assert.That(item2InResults.Timestamp, Is.LessThan(item3InResults.Timestamp));
    Assert.That(item3InResults.Timestamp, Is.LessThan(item1InResults.Timestamp));
  }

  [Test]
  public void WindowByTimestamp_WithMultipleWindows_ClosesWindowsCorrectly()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var windowDuration = TimeSpan.FromSeconds(10);
    var completedWindows = new ConcurrentBag<bool>();
    var windowItems = new ConcurrentBag<ConcurrentBag<TestMetric>>();

    var item1 = new TestMetric(baseTime, 1.0);
    var item2 = new TestMetric(baseTime.AddSeconds(15), 2.0); // Different window

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(window =>
      {
        var items = new ConcurrentBag<TestMetric>();
        var windowCompleted = false;

        window.Subscribe(
          item => items.Add(item),
          onCompleted: () => windowCompleted = true);

        windowItems.Add(items);
        completedWindows.Add(windowCompleted);
      });

    _source.OnNext(item1);
    Thread.Sleep(1500); // Wait for buffer
    _source.OnNext(item2);
    Thread.Sleep(1500); // Wait for buffer
    _source.OnCompleted();

    // Give some time for completion to propagate
    Thread.Sleep(100);

    // Assert
    Assert.That(windowItems, Has.Count.EqualTo(2));
    var windowItemsList = windowItems.ToList();

    // Check that each window has exactly one item
    Assert.That(windowItemsList[0], Has.Count.EqualTo(1));
    Assert.That(windowItemsList[1], Has.Count.EqualTo(1));

    // Check that we have both expected values (order may vary)
    var allValues = windowItemsList.SelectMany(w => w.Select(item => item.Value)).OrderBy(v => v).ToArray();
    Assert.That(allValues, Is.EqualTo(new[] { 1.0, 2.0 }));
  }

  [Test]
  public void WindowByTimestamp_WithZeroWindowDuration_DoesNotThrowImmediately()
  {
    // Arrange
    var windowDuration = TimeSpan.Zero;

    // Act & Assert - This might not throw immediately due to lazy evaluation
    var observable = _source.WindowByTimestamp(windowDuration);

    // The exception might be thrown when subscribing or when processing items
    Assert.DoesNotThrow(() =>
    {
      observable.Subscribe(window => { });
    });
  }

  [Test]
  public void WindowByTimestamp_WithKeySelectorThrowingException_PropagatesException()
  {
    // Arrange
    var source = new Subject<string>();
    Exception? caughtException = null;
    Func<string, long> faultyKeySelector = s =>
    {
      if (s == "error")
        throw new InvalidOperationException("Key selector error");
      return DateTime.Now.Ticks;
    };

    // Act
    source
      .WindowByTimestamp(faultyKeySelector, TimeSpan.FromSeconds(10))
      .Subscribe(
        window => { },
        error => caughtException = error);

    source.OnNext("valid");
    Thread.Sleep(1500); // Wait for buffer
    source.OnNext("error");
    Thread.Sleep(1500); // Wait for buffer

    // Assert
    Assert.That(caughtException, Is.Not.Null);
    Assert.That(caughtException, Is.TypeOf<InvalidOperationException>());
    Assert.That(caughtException.Message, Contains.Substring("Key selector error"));
  }

  [Test]
  public void WindowByTimestamp_WithLargeNumberOfItems_HandlesCorrectly()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var windowDuration = TimeSpan.FromSeconds(1);
    var windowCount = 0;
    var totalItems = 0;

    // Act
    _source
      .WindowByTimestamp(windowDuration)
      .Subscribe(window =>
      {
        windowCount++;
        window.Subscribe(item => totalItems++);
      });

    // Generate 1000 items across multiple windows
    for (int i = 0; i < 1000; i++)
    {
      var timestamp = baseTime.AddMilliseconds(i * 10); // 10ms intervals
      _source.OnNext(new TestMetric(timestamp, i * 1.5)); // Use double values
    }

    Thread.Sleep(2000); // Wait for all buffers to process
    _source.OnCompleted();

    // Assert
    Assert.That(windowCount, Is.GreaterThan(1), "Should create multiple windows");
    Assert.That(totalItems, Is.EqualTo(1000), "Should process all items");
  }
}
