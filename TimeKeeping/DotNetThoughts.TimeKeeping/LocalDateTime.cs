namespace DotNetThoughts.TimeKeeping;

/// <summary>
/// Unambigious representation of a local datetime
/// </summary>
public readonly record struct LocalDateTime
{
    public LocalDateTime(DateOnly date, TimeZoneInfo timeZoneInfo)
        : this(date.ToDateTime(TimeOnly.MinValue), timeZoneInfo) { }

    public LocalDateTime(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        if (!((dateTime.Kind == DateTimeKind.Local && timeZoneInfo == TimeZoneInfo.Local)
            || (dateTime.Kind == DateTimeKind.Utc && timeZoneInfo == TimeZoneInfo.Utc)
            || (dateTime.Kind == DateTimeKind.Unspecified && timeZoneInfo != TimeZoneInfo.Local)))
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

    public LocalDateTime ToTimeZone(TimeZoneInfo newTz)
    {
        return new LocalDateTime(TimeZoneInfo.ConvertTime(DateTime, TimeZoneInfo, newTz), newTz);
    }

    public LocalDateTime ToMidnight()
    {
        return new LocalDateTime(DateTime.Date, TimeZoneInfo);
    }

    public LocalDateTime ToBeginningOfYear()
    {
        return new LocalDateTime(new DateTime(DateTime.Year, 1, 1), TimeZoneInfo);
    }

    public LocalDateTime ToEndOfYear()
    {
        return new LocalDateTime(new DateTime(DateTime.Year, 12, 31, 23, 59, 59, 999, 999), TimeZoneInfo);
    }

    public DateTimeOffset ToDateTimeOffsetUtc()
    {
        var shouldHaveHadKindUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo);
        return shouldHaveHadKindUtc;
    }

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
