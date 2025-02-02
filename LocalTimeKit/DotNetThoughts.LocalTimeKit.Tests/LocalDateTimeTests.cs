namespace DotNetThoughts.LocalTimeKit.Tests;
public class LocalDateTimeTests
{

    [Fact]
    public void TestLocalDateTimeOffset()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var expectedDateTimeOffset = DateTimeOffset.Parse("2024-06-06T00:00:00+02:00");
        ldt.ToDateTimeOffsetLocal().ShouldBe(expectedDateTimeOffset);
    }

    [Fact]
    public void ConvertBetweenTimeZones_Utc()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var utc = ldt.ToTimeZone(TimeZoneInfo.Utc);
        utc.DateTime.Kind.ShouldBe(DateTimeKind.Utc);
        utc.ToDateTimeOffsetUtc().ShouldBe(ldt.ToDateTimeOffsetUtc());
    }

    [Fact]
    public void UtcIsLocalUtc()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Utc);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        var ldt = new LocalDateTime(t, tz);
        ldt.ToDateTimeOffsetLocal().ShouldBe(ldt.ToDateTimeOffsetUtc());
    }

    [Fact]
    public void ConvertBetweenTimeZones_Hawaii1()
    {
        var t = new DateTime(2024, 06, 17, 22, 24, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var hawaii = ldt.ToTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time"));
        hawaii.DateTime.Kind.ShouldBe(DateTimeKind.Unspecified);
        hawaii.ToDateTimeOffsetUtc().ShouldBe(ldt.ToDateTimeOffsetUtc());
        hawaii.ToDateTimeOffsetLocal().ToString("yyyy-MM-ddTHH:mm:ssK").ShouldBe("2024-06-17T10:24:00-10:00");
    }

    [Fact]
    public void ConvertBetweenTimeZones_Hawaii2()
    {
        var t = new DateTime(2024, 02, 17, 22, 24, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var hawaii = ldt.ToTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time"));
        hawaii.DateTime.Kind.ShouldBe(DateTimeKind.Unspecified);
        hawaii.ToDateTimeOffsetUtc().ShouldBe(ldt.ToDateTimeOffsetUtc());
        hawaii.ToDateTimeOffsetLocal().ToString("yyyy-MM-ddTHH:mm:ssK").ShouldBe("2024-02-17T11:24:00-10:00");
    }

    // 
    [Fact]
    public void TestDateConstructor()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var d = new DateOnly(2024, 06, 06);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var ldt2 = new LocalDateTime(d, tz);

        ldt.DateTime.ShouldBe(ldt2.DateTime);
    }

    [Fact]
    public void TestDateTimeOffsetConstructor()
    {
        var t = new DateTimeOffset(2024, 06, 06, 0, 0, 0, TimeSpan.FromHours(2));
        var d = new DateOnly(2024, 06, 06);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var ldt2 = new LocalDateTime(d, tz);

        ldt.DateTime.ShouldBe(ldt2.DateTime);
    }

    [Fact]
    public void TestUtcDateTimeOffset()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var expectedUtcDateTimeOffset = DateTimeOffset.Parse("2024-06-05T22:00:00+00:00");
        ldt.ToDateTimeOffsetUtc().ShouldBe(expectedUtcDateTimeOffset);
    }

    [Fact]
    public void InTheMiddleOfSummerTimeShift()
    {
        var t = new DateTime(2024, 03, 31, 2, 30, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        Action ldt = () => new LocalDateTime(t, tz);
        ldt.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void InTheMiddleOfWinterTimeShift()
    {
        var t = new DateTime(2024, 10, 27, 2, 30, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        Action ldt = () => new LocalDateTime(t, tz);
        ldt.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void TestLocalMidnight()
    {
        var t = new DateTime(2024, 06, 06, 13, 2, 1, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz).ToMidnight();
        var expectedDateTimeOffset = DateTimeOffset.Parse("2024-06-06T00:00:00+02:00");
        ldt.ToDateTimeOffsetLocal().ShouldBe(expectedDateTimeOffset);
    }

    [Fact]
    public void LocalTimeKindaWorks()
    {
        var nowInContextOfTest = DateTimeOffset.Now;

        var t = new DateTime(nowInContextOfTest.Ticks, DateTimeKind.Local);
        var tz = TimeZoneInfo.Local;
        var ldt = new LocalDateTime(t, tz);
        ldt.ToDateTimeOffsetLocal().ShouldBe(nowInContextOfTest);
    }

    [Fact]
    public void LocalTimeKindaWorksWithUtc()
    {
        var nowInContextOfTest = DateTimeOffset.Now;
        var utcNow = nowInContextOfTest.ToUniversalTime();
        var t = new DateTime(nowInContextOfTest.Ticks, DateTimeKind.Local);
        var tz = TimeZoneInfo.Local;
        var ldt = new LocalDateTime(t, tz);
        ldt.ToDateTimeOffsetUtc().ShouldBe(utcNow);
    }

    [Theory]
    [InlineData("2022-01-01T16:00:00", 1, "2022-01-01T00:00:00", "2022-01-01T00:00:00+01:00", "2021-12-31T23:00:00+00:00")]
    [InlineData("2022-01-01T00:00:00", 1, "2021-12-31T00:00:00", "2021-12-31T00:00:00+01:00", "2021-12-30T23:00:00+00:00")]
    [InlineData("2022-01-01T16:00:00", 2, "2021-12-31T00:00:00", "2021-12-31T00:00:00+01:00", "2021-12-30T23:00:00+00:00")]
    [InlineData("2024-04-05T16:00:00", 7, "2024-03-30T00:00:00", "2024-03-30T00:00:00+01:00", "2024-03-29T23:00:00+00:00")]
    [InlineData("2024-10-28T16:00:00", 3, "2024-10-26T00:00:00", "2024-10-26T00:00:00+02:00", "2024-10-25T22:00:00+00:00")]
    public void PastMidnightsTest(string date, int midnights, string expected, string expectedLocalOffset, string expectedUtcOffset)
    {
        var t = DateTime.Parse(date);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var past = ldt.PastMidnights(midnights);
        past.ShouldBe(new LocalDateTime(DateTime.Parse(expected), tz));
        past.ToDateTimeOffsetLocal().ShouldBe(DateTimeOffset.Parse(expectedLocalOffset));
        past.ToDateTimeOffsetUtc().ShouldBe(DateTimeOffset.Parse(expectedUtcOffset));
    }

    [Theory]
    [InlineData("2022-01-01T16:00:00", 1, "2022-01-02T00:00:00", "2022-01-02T00:00:00+01:00", "2022-01-01T23:00:00+00:00")]
    [InlineData("2022-01-01T00:00:00", 1, "2022-01-02T00:00:00", "2022-01-02T00:00:00+01:00", "2022-01-01T23:00:00+00:00")]
    [InlineData("2022-01-01T16:00:00", 2, "2022-01-03T00:00:00", "2022-01-03T00:00:00+01:00", "2022-01-02T23:00:00+00:00")]
    [InlineData("2024-03-30T16:00:00", 6, "2024-04-05T00:00:00", "2024-04-05T00:00:00+02:00", "2024-04-04T22:00:00+00:00")]
    [InlineData("2024-10-26T16:00:00", 2, "2024-10-28T00:00:00", "2024-10-28T00:00:00+01:00", "2024-10-27T23:00:00+00:00")]
    public void FutureMidnightsTest(string date, int midnights, string expected, string expectedLocalOffset, string expectedUtcOffset)
    {
        var t = DateTime.Parse(date);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var future = ldt.FutureMidnights(midnights);
        future.ShouldBe(new LocalDateTime(DateTime.Parse(expected), tz));
        future.ToDateTimeOffsetLocal().ShouldBe(DateTimeOffset.Parse(expectedLocalOffset));
        future.ToDateTimeOffsetUtc().ShouldBe(DateTimeOffset.Parse(expectedUtcOffset));

    }
}
