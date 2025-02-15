using TUnit.Assertions.AssertConditions.Throws;

namespace DotNetThoughts.LocalTimeKit.Tests;
public class LocalDateTimeTests
{

    [Test]
    public async Task TestLocalDateTimeOffset()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var expectedDateTimeOffset = DateTimeOffset.Parse("2024-06-06T00:00:00+02:00");
        await Assert.That(ldt.ToDateTimeOffsetLocal()).IsEqualTo(expectedDateTimeOffset);
    }

    [Test]
    public async Task ConvertBetweenTimeZones_Utc()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var utc = ldt.ToTimeZone(TimeZoneInfo.Utc);
        await Assert.That(utc.DateTime.Kind).IsEqualTo(DateTimeKind.Utc);
        await Assert.That(utc.ToDateTimeOffsetUtc()).IsEqualTo(ldt.ToDateTimeOffsetUtc());
    }

    [Test]
    public async Task UtcIsLocalUtc()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Utc);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        var ldt = new LocalDateTime(t, tz);
        await Assert.That(ldt.ToDateTimeOffsetLocal()).IsEqualTo(ldt.ToDateTimeOffsetUtc());
    }

    [Test]
    public async Task ConvertBetweenTimeZones_Hawaii1()
    {
        var t = new DateTime(2024, 06, 17, 22, 24, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var hawaii = ldt.ToTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time"));
        await Assert.That(hawaii.DateTime.Kind).IsEqualTo(DateTimeKind.Unspecified);
        await Assert.That(hawaii.ToDateTimeOffsetUtc()).IsEqualTo(ldt.ToDateTimeOffsetUtc());
        await Assert.That(hawaii.ToDateTimeOffsetLocal().ToString("yyyy-MM-ddTHH:mm:ssK")).IsEqualTo("2024-06-17T10:24:00-10:00");
    }

    [Test]
    public async Task ConvertBetweenTimeZones_Hawaii2()
    {
        var t = new DateTime(2024, 02, 17, 22, 24, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var hawaii = ldt.ToTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time"));
        await Assert.That(hawaii.DateTime.Kind).IsEqualTo(DateTimeKind.Unspecified);
        await Assert.That(hawaii.ToDateTimeOffsetUtc()).IsEqualTo(ldt.ToDateTimeOffsetUtc());
        await Assert.That(hawaii.ToDateTimeOffsetLocal().ToString("yyyy-MM-ddTHH:mm:ssK")).IsEqualTo("2024-02-17T11:24:00-10:00");
    }

    [Test]
    public async Task TestDateConstructor()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var d = new DateOnly(2024, 06, 06);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var ldt2 = new LocalDateTime(d, tz);
        await Assert.That(ldt.DateTime).IsEqualTo(ldt2.DateTime);
    }

    [Test]
    public async Task TestDateTimeOffsetConstructor()
    {
        var t = new DateTimeOffset(2024, 06, 06, 0, 0, 0, TimeSpan.FromHours(2));
        var d = new DateOnly(2024, 06, 06);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var ldt2 = new LocalDateTime(d, tz);
        await Assert.That(ldt.DateTime).IsEqualTo(ldt2.DateTime);
    }

    [Test]
    public async Task TestUtcDateTimeOffset()
    {
        var t = new DateTime(2024, 06, 06, 0, 0, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var expectedUtcDateTimeOffset = DateTimeOffset.Parse("2024-06-05T22:00:00+00:00");
        await Assert.That(ldt.ToDateTimeOffsetUtc()).IsEqualTo(expectedUtcDateTimeOffset);
    }

    [Test]
    public async Task InTheMiddleOfSummerTimeShift()
    {
        var t = new DateTime(2024, 03, 31, 2, 30, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        Action ldt = () => new LocalDateTime(t, tz);
        await Assert.That(ldt).ThrowsExactly<ArgumentException>();
    }

    [Test]
    public async Task InTheMiddleOfWinterTimeShift()
    {
        var t = new DateTime(2024, 10, 27, 2, 30, 0, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        Action ldt = () => new LocalDateTime(t, tz);
        await Assert.That(ldt).ThrowsExactly<ArgumentException>();
    }

    [Test]
    public async Task TestLocalMidnight()
    {
        var t = new DateTime(2024, 06, 06, 13, 2, 1, DateTimeKind.Unspecified);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz).ToMidnight();
        var expectedDateTimeOffset = DateTimeOffset.Parse("2024-06-06T00:00:00+02:00");
        await Assert.That(ldt.ToDateTimeOffsetLocal()).IsEqualTo(expectedDateTimeOffset);
    }

    [Test]
    public async Task LocalTimeKindaWorks()
    {
        var nowInContextOfTest = DateTimeOffset.Now;

        var t = new DateTime(nowInContextOfTest.Ticks, DateTimeKind.Local);
        var tz = TimeZoneInfo.Local;
        var ldt = new LocalDateTime(t, tz);
        await Assert.That(ldt.ToDateTimeOffsetLocal()).IsEqualTo(nowInContextOfTest);
    }

    [Test]
    public async Task LocalTimeKindaWorksWithUtc()
    {
        var nowInContextOfTest = DateTimeOffset.Now;
        var utcNow = nowInContextOfTest.ToUniversalTime();
        var t = new DateTime(nowInContextOfTest.Ticks, DateTimeKind.Local);
        var tz = TimeZoneInfo.Local;
        var ldt = new LocalDateTime(t, tz);
        await Assert.That(ldt.ToDateTimeOffsetUtc()).IsEqualTo(utcNow);
    }

    [Test]
    [Arguments("2022-01-01T16:00:00", 1, "2022-01-01T00:00:00", "2022-01-01T00:00:00+01:00", "2021-12-31T23:00:00+00:00")]
    [Arguments("2022-01-01T00:00:00", 1, "2021-12-31T00:00:00", "2021-12-31T00:00:00+01:00", "2021-12-30T23:00:00+00:00")]
    [Arguments("2022-01-01T16:00:00", 2, "2021-12-31T00:00:00", "2021-12-31T00:00:00+01:00", "2021-12-30T23:00:00+00:00")]
    [Arguments("2024-04-05T16:00:00", 7, "2024-03-30T00:00:00", "2024-03-30T00:00:00+01:00", "2024-03-29T23:00:00+00:00")]
    [Arguments("2024-10-28T16:00:00", 3, "2024-10-26T00:00:00", "2024-10-26T00:00:00+02:00", "2024-10-25T22:00:00+00:00")]
    public async Task PastMidnightsTest(string date, int midnights, string expected, string expectedLocalOffset, string expectedUtcOffset)
    {
        var t = DateTime.Parse(date);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var past = ldt.PastMidnights(midnights);
        await Assert.That(past).IsEqualTo(new LocalDateTime(DateTime.Parse(expected), tz));
        await Assert.That(past.ToDateTimeOffsetLocal()).IsEqualTo(DateTimeOffset.Parse(expectedLocalOffset));
        await Assert.That(past.ToDateTimeOffsetUtc()).IsEqualTo(DateTimeOffset.Parse(expectedUtcOffset));
    }

    [Test]
    [Arguments("2022-01-01T16:00:00", 1, "2022-01-02T00:00:00", "2022-01-02T00:00:00+01:00", "2022-01-01T23:00:00+00:00")]
    [Arguments("2022-01-01T00:00:00", 1, "2022-01-02T00:00:00", "2022-01-02T00:00:00+01:00", "2022-01-01T23:00:00+00:00")]
    [Arguments("2022-01-01T16:00:00", 2, "2022-01-03T00:00:00", "2022-01-03T00:00:00+01:00", "2022-01-02T23:00:00+00:00")]
    [Arguments("2024-03-30T16:00:00", 6, "2024-04-05T00:00:00", "2024-04-05T00:00:00+02:00", "2024-04-04T22:00:00+00:00")]
    [Arguments("2024-10-26T16:00:00", 2, "2024-10-28T00:00:00", "2024-10-28T00:00:00+01:00", "2024-10-27T23:00:00+00:00")]
    public async Task FutureMidnightsTest(string date, int midnights, string expected, string expectedLocalOffset, string expectedUtcOffset)
    {
        var t = DateTime.Parse(date);
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var ldt = new LocalDateTime(t, tz);
        var future = ldt.FutureMidnights(midnights);

        await Assert.That(future).IsEqualTo(new LocalDateTime(DateTime.Parse(expected), tz));
        await Assert.That(future.ToDateTimeOffsetLocal()).IsEqualTo(DateTimeOffset.Parse(expectedLocalOffset));
        await Assert.That(future.ToDateTimeOffsetUtc()).IsEqualTo(DateTimeOffset.Parse(expectedUtcOffset));
    }
}
