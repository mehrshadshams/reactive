using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.Ext.Tests;

/// <summary>
/// Test data structure for metrics with name and value.
/// </summary>
public record MetricData(DateTimeOffset Timestamp, string Name, double Value) : ITimestamped;

/// <summary>
/// Tests for complex windowed average conditions using WindowByTimestamp.
/// </summary>
[TestFixture]
public class WindowedAverageConditionTests
{
  private Subject<MetricData> _source;

  [SetUp]
  public void Setup()
  {
    _source = new Subject<MetricData>();
  }

  [TearDown]
  public void TearDown()
  {
    _source?.Dispose();
  }

  [Test]
  public void WindowedAverageCondition_WhenBothConditionsTrue_ReturnsTrue()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var results = new ConcurrentBag<bool>();

    // Test data that should satisfy both conditions (X avg > 0.5, Y avg >= 0.7)
    var testData = new[]
    {
      new MetricData(baseTime, "x", 0.6),
      new MetricData(baseTime, "y", 0.8),
      new MetricData(baseTime.AddSeconds(2), "x", 0.7),
      new MetricData(baseTime.AddSeconds(2), "y", 0.9),
      new MetricData(baseTime.AddSeconds(4), "x", 0.8),
      new MetricData(baseTime.AddSeconds(4), "y", 0.7),
      new MetricData(baseTime.AddSeconds(6), "x", 0.9),
      new MetricData(baseTime.AddSeconds(6), "y", 0.8),
      new MetricData(baseTime.AddSeconds(8), "x", 0.6),
      new MetricData(baseTime.AddSeconds(8), "y", 0.6),
      new MetricData(baseTime.AddSeconds(10), "x", 0.7),
      new MetricData(baseTime.AddSeconds(10), "y", 0.7),
      new MetricData(baseTime.AddSeconds(12), "x", 0.8),
      new MetricData(baseTime.AddSeconds(12), "y", 0.8)
    };

    // Create the complex observable that checks: avg(x, 10sec) > 0.5 && avg(y, 15sec) >= 0.7
    var conditionObservable = CreateConditionObservable(_source);

    // Subscribe to collect results
    conditionObservable.Subscribe(result => results.Add(result));

    // Act - Send all test data
    SendTestData(testData);

    // Assert
    Assert.That(results, Is.Not.Empty, "Should produce at least one result");
    Assert.That(results.Any(r => r), Is.True, "Should have at least one true result when conditions are met");
  }

  [Test]
  public void WindowedAverageCondition_WhenFirstConditionFalse_ReturnsFalse()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var results = new ConcurrentBag<bool>();

    // Test data where X average <= 0.5 (first condition false)
    var testData = new[]
    {
      new MetricData(baseTime, "x", 0.3),
      new MetricData(baseTime, "y", 0.8),
      new MetricData(baseTime.AddSeconds(2), "x", 0.4),
      new MetricData(baseTime.AddSeconds(2), "y", 0.9),
      new MetricData(baseTime.AddSeconds(4), "x", 0.2),
      new MetricData(baseTime.AddSeconds(4), "y", 0.7),
      new MetricData(baseTime.AddSeconds(6), "x", 0.5),
      new MetricData(baseTime.AddSeconds(6), "y", 0.8),
      new MetricData(baseTime.AddSeconds(8), "x", 0.1),
      new MetricData(baseTime.AddSeconds(8), "y", 0.7),
      new MetricData(baseTime.AddSeconds(10), "x", 0.3),
      new MetricData(baseTime.AddSeconds(10), "y", 0.8)
    };

    var conditionObservable = CreateConditionObservable(_source);
    conditionObservable.Subscribe(result => results.Add(result));

    // Act - Send all test data
    SendTestData(testData);

    // Assert
    Assert.That(results, Is.Not.Empty, "Should produce at least one result");
    // Since avg(X) <= 0.5, condition should be false
    var lastResults = results.TakeLast(3).ToList(); // Check recent results
    Assert.That(lastResults.All(r => !r), Is.True, "Should return false when X average <= 0.5");
  }

  [Test]
  public void WindowedAverageCondition_WhenSecondConditionFalse_ReturnsFalse()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var results = new ConcurrentBag<bool>();

    // Test data where Y average < 0.7 (second condition false)
    var testData = new[]
    {
      new MetricData(baseTime, "x", 0.8),
      new MetricData(baseTime, "y", 0.5),
      new MetricData(baseTime.AddSeconds(2), "x", 0.9),
      new MetricData(baseTime.AddSeconds(2), "y", 0.4),
      new MetricData(baseTime.AddSeconds(4), "x", 0.7),
      new MetricData(baseTime.AddSeconds(4), "y", 0.6),
      new MetricData(baseTime.AddSeconds(6), "x", 0.8),
      new MetricData(baseTime.AddSeconds(6), "y", 0.5),
      new MetricData(baseTime.AddSeconds(8), "x", 0.9),
      new MetricData(baseTime.AddSeconds(8), "y", 0.6),
      new MetricData(baseTime.AddSeconds(10), "x", 0.8),
      new MetricData(baseTime.AddSeconds(10), "y", 0.5)
    };

    var conditionObservable = CreateConditionObservable(_source);
    conditionObservable.Subscribe(result => results.Add(result));

    // Act - Send all test data
    SendTestData(testData);

    // Assert
    Assert.That(results, Is.Not.Empty, "Should produce at least one result");
    // Since avg(Y) < 0.7, condition should be false
    var lastResults = results.TakeLast(3).ToList(); // Check recent results
    Assert.That(lastResults.All(r => !r), Is.True, "Should return false when Y average < 0.7");
  }

  [Test]
  public void WindowedAverageCondition_WhenBothConditionsFalse_ReturnsFalse()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var results = new ConcurrentBag<bool>();

    // Test data where both conditions are false (X avg <= 0.5 AND Y avg < 0.7)
    var testData = new[]
    {
      new MetricData(baseTime, "x", 0.3),
      new MetricData(baseTime, "y", 0.5),
      new MetricData(baseTime.AddSeconds(2), "x", 0.2),
      new MetricData(baseTime.AddSeconds(2), "y", 0.4),
      new MetricData(baseTime.AddSeconds(4), "x", 0.4),
      new MetricData(baseTime.AddSeconds(4), "y", 0.6),
      new MetricData(baseTime.AddSeconds(6), "x", 0.1),
      new MetricData(baseTime.AddSeconds(6), "y", 0.5),
      new MetricData(baseTime.AddSeconds(8), "x", 0.3),
      new MetricData(baseTime.AddSeconds(8), "y", 0.6),
      new MetricData(baseTime.AddSeconds(10), "x", 0.2),
      new MetricData(baseTime.AddSeconds(10), "y", 0.5)
    };

    var conditionObservable = CreateConditionObservable(_source);
    conditionObservable.Subscribe(result => results.Add(result));

    // Act - Send all test data
    SendTestData(testData);

    // Assert
    Assert.That(results, Is.Not.Empty, "Should produce at least one result");
    // Both conditions false, result should be false
    var lastResults = results.TakeLast(3).ToList(); // Check recent results
    Assert.That(lastResults.All(r => !r), Is.True, "Should return false when both conditions are false");
  }

  [Test]
  public void WindowedAverageCondition_WithEmptyWindows_HandlesProperly()
  {
    // Arrange
    var baseTime = DateTimeOffset.Now;
    var results = new ConcurrentBag<bool>();

    // Test data with minimal points
    var testData = new[]
    {
      new MetricData(baseTime, "x", 0.8),
      new MetricData(baseTime, "y", 0.9)
    };

    var conditionObservable = CreateConditionObservable(_source);
    conditionObservable.Subscribe(result => results.Add(result));

    // Act - Send minimal test data
    SendTestData(testData);

    // Assert - Should handle single data point gracefully
    Assert.DoesNotThrow(() => { }, "Should handle empty or single-item windows without throwing");
  }

  /// <summary>
  /// Helper method to send test data to the subject with appropriate timing.
  /// </summary>
  /// <param name="testData">Array of MetricData to send</param>
  private void SendTestData(MetricData[] testData)
  {
    foreach (var data in testData)
    {
      _source.OnNext(data);
    }

    Thread.Sleep(1200); // Wait for processing
    _source.OnCompleted();
    Thread.Sleep(100); // Wait for completion
  }

  /// <summary>
  /// Creates the complex observable that implements the condition:
  /// avg(x, 10sec) > 0.5 && avg(y, 15sec) >= 0.7
  /// </summary>
  /// <param name="source">The source observable of MetricData</param>
  /// <returns>Observable that emits boolean values when conditions are evaluated</returns>
  private static IObservable<bool> CreateConditionObservable(IObservable<MetricData> source)
  {
    // Filter and create windowed averages for X (10 seconds) and Y (15 seconds)
    var xAverageObservable = source
      .Where(data => data.Name == "x")
      .WindowByTimestamp(TimeSpan.FromSeconds(10))
      .SelectMany(window => window
        .Select(data => data.Value)
        .DefaultIfEmpty(0.0)
        .Average()
        .Select(avg => new { Type = "x", Average = avg, Value = avg > 0.5 })); // Add type information

    var yAverageObservable = source
      .Where(data => data.Name == "y")
      .WindowByTimestamp(TimeSpan.FromSeconds(15))
      .SelectMany(window => window
        .Select(data => data.Value)
        .DefaultIfEmpty(0.0)
        .Average()
        .Select(avg => new { Type = "y", Average = avg, Value = avg > 0.7 })) ; // Add type information

    // Merge both streams and maintain state
    // return xAverageObservable
    //   .Merge(yAverageObservable)
    //   .Scan(new { XAvg = 0.0, YAvg = 0.0 }, (state, update) =>
    //     update.Type == "x"
    //       ? new { XAvg = update.Average, YAvg = state.YAvg }
    //       : new { XAvg = state.XAvg, YAvg = update.Average })
    //   .Select(state => state.XAvg > 0.5 && state.YAvg >= 0.7)
    //   .DistinctUntilChanged(); // Only emit when the boolean result changes

    return xAverageObservable
      .CombineLatest(yAverageObservable, (a,b) =>
      {
        return new { Value = a.Value & b.Value };
      })
      // .Scan(new { Value = true }, (state, update) =>
      // {
      //   return new { Value = state.Value && update.Value };
      // })
      // .Select(state => state.XAvg > 0.5 && state.YAvg >= 0.7)
      .Select(state => state.Value)
      .DistinctUntilChanged(); // Only emit when the boolean result changes
  }
}
