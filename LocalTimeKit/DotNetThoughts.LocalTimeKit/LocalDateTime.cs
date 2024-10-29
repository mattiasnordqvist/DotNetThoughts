namespace DotNetThoughts.LocalTimeKit;

/// <summary>
/// Unambigious representation of a local datetime
/// </summary>
public readonly record struct LocalDateTime
{
    /// <summary>
    /// Creates a LocalDateTime from a DateOnly and a TimeZoneInfo
    /// The date represents midnight in the given timezone
    /// 
    /// See <see cref="LocalDateTime(DateTime, TimeZoneInfo)"/>
    /// </summary>
    /// <param name="date"></param>
    /// <param name="timeZoneInfo"></param>
    public LocalDateTime(DateOnly date, TimeZoneInfo timeZoneInfo)
        : this(date.ToDateTime(TimeOnly.MinValue), timeZoneInfo) { }

    /// <summary>
    /// Creates a LocalDateTime from a DateTimeOffset and a TimeZoneInfo
    /// 
    /// See <see cref="LocalDateTime(DateTime, TimeZoneInfo)"/>
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="timeZoneInfo"></param>
    public LocalDateTime(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
        : this(TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo).DateTime, timeZoneInfo) { }

    /// <summary>
    /// Creates a LocalDateTime from a DateTime and a TimeZoneInfo
    /// If the datetime has DateTimeKind.Local, the timezone must be TimeZoneInfo.Local
    /// If the datetime has DateTimeKind.Utc, the timezone must be TimeZoneInfo.Utc
    /// Otherwise, the timezone must not be TimeZoneInfo.Local
    /// In any of the cases above, an ArgumentException is thrown.
    /// 
    /// Some times are invalid for some timezones, and some times are ambiguous.
    /// Any such time and timezone combination will throw an ArgumentException.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="timeZoneInfo"></param>
    /// <exception cref="ArgumentException"></exception>
    public LocalDateTime(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        if (!(dateTime.Kind == DateTimeKind.Local && timeZoneInfo == TimeZoneInfo.Local
            || dateTime.Kind == DateTimeKind.Utc && timeZoneInfo == TimeZoneInfo.Utc
            || dateTime.Kind == DateTimeKind.Unspecified && timeZoneInfo != TimeZoneInfo.Local))
        {
            throw new ArgumentException("Invalid combinations of datetime kinds and timezoneinfo");
        }

        if (timeZoneInfo.IsInvalidTime(dateTime))
        {
            throw new ArgumentException("dateTime is invalid for the timezone");
        }

        if (timeZoneInfo.IsAmbiguousTime(dateTime))
        {
            throw new ArgumentException("dateTime is ambiguous for the timezone");
        }

        DateTime = dateTime;
        TimeZoneInfo = timeZoneInfo;
    }

    public DateTime DateTime { get; }
    public TimeZoneInfo TimeZoneInfo { get; }

    /// <summary>
    /// Returns a new LocalDateTime representing the same instance in time, but in relation to a different time zone.
    /// </summary>
    /// <param name="newTz"></param>
    /// <returns></returns>
    public LocalDateTime ToTimeZone(TimeZoneInfo newTz)
    {
        return new LocalDateTime(TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, newTz), newTz);
    }

    /// <summary>
    /// Returns a new LocalDateTime representing the midnight of the current date.
    /// </summary>
    /// <returns></returns>
    public LocalDateTime ToMidnight()
    {
        return new LocalDateTime(DateTime.Date, TimeZoneInfo);
    }

    /// <summary>
    /// Returns a new LocalDateTime representing the beginning of the current year.
    /// </summary>
    /// <returns></returns>
    public LocalDateTime ToBeginningOfYear()
    {
        return new LocalDateTime(new DateTime(DateTime.Year, 1, 1), TimeZoneInfo);
    }

    /// <summary>
    /// Returns a new LocalDateTime representing the end of the current year.
    /// </summary>
    /// <returns></returns>
    public LocalDateTime ToEndOfYear()
    {
        return new LocalDateTime(new DateTime(DateTime.Year, 12, 31, 23, 59, 59, 999, 999), TimeZoneInfo);
    }

    /// <summary>
    /// Returns a DateTimeOffset, with offset 0, representing this instance in time.
    /// </summary>
    /// <returns></returns>
    public DateTimeOffset ToDateTimeOffsetUtc()
    {
        var shouldHaveHadKindUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo);
        return shouldHaveHadKindUtc;
    }

    /// <summary>
    /// Returns a DateTimeOffset, with offset matching the current timezone, representing this instance in time.
    /// </summary>
    /// <returns></returns>
    public DateTimeOffset ToDateTimeOffsetLocal()
    {
        return new DateTimeOffset(DateTime, TimeZoneInfo.GetUtcOffset(DateTime));
    }

    /// <summary>
    /// Imagine past occurrences of midnight as a 1-indexed array where the first occurence is the last midnight experienced. 
    /// </summary>
    public LocalDateTime PastMidnights(int count)
    {
        if (count < 1)
        {
            throw new ArgumentException("Count must be greater than 0.");
        }

        if (DateTime.TimeOfDay == TimeSpan.Zero)
        {
            return new LocalDateTime(DateTime.AddDays(-count), TimeZoneInfo).ToMidnight();
        }
        else
        {
            return new LocalDateTime(DateTime.AddDays(-(count - 1)), TimeZoneInfo).ToMidnight();
        }
    }

    /// <summary>
    /// Imagine future occurrences of midnight as a 1-indexed array where the first occurence is the next midnight experienced. 
    /// </summary>
    public LocalDateTime FutureMidnights(int count)
    {
        if (count < 1)
        {
            throw new ArgumentException("Count must be greater than 0.");
        }

        return new LocalDateTime(DateTime.AddDays(count), TimeZoneInfo).ToMidnight();
    }

}
