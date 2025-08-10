namespace Reactive.Expressions;

public static class DateTimeExtensions
{
  public static DateTimeOffset Truncate(this DateTimeOffset dateTime, TimeSpan timeSpan)
  {
    int minute = timeSpan.Minutes > 0 ? timeSpan.Minutes * (dateTime.Minute / timeSpan.Minutes) : dateTime.Minute;
    int hour = timeSpan.Hours > 0 ? timeSpan.Hours * (dateTime.Hour / timeSpan.Hours) : dateTime.Hour;
    int day = timeSpan.Days > 0 ? timeSpan.Days * (dateTime.Day / timeSpan.Days) : dateTime.Day;
    return new DateTimeOffset(dateTime.Year, dateTime.Month, day, hour, minute, 0, dateTime.Offset);
  }

  public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
  {
    int minute = timeSpan.Minutes > 0 ? timeSpan.Minutes * (dateTime.Minute / timeSpan.Minutes) : dateTime.Minute;
    int hour = timeSpan.Hours > 0 ? timeSpan.Hours * (dateTime.Hour / timeSpan.Hours) : dateTime.Hour;
    int day = timeSpan.Days > 0 ? timeSpan.Days * (dateTime.Day / timeSpan.Days) : dateTime.Day;
    return new DateTime(dateTime.Year, dateTime.Month, day, hour, minute, 0);
  }
}
