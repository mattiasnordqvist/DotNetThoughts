using System.Diagnostics;

namespace DotNetThoughts.TimeKeeping.Tests;
public class TimeTravelersClockTests
{
    private static TimeSpan _allowedDeviation = TimeSpan.FromMilliseconds(100);
    [Fact]
    public void FreezeTime()
    {
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var sut = new TimeTravelersClock();
        sut.Freeze(frozenTime);
        sut.IsFrozen().Should().BeTrue();
        sut.Now().Should().Be(frozenTime);
    }
    [Fact]
    public void FreezeAgainOverridesCurrentFreeze()
    {
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newFrozenTime = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var sut = new TimeTravelersClock();
        sut.Freeze(frozenTime);
        sut.Freeze(newFrozenTime);
        sut.IsFrozen().Should().BeTrue();
        sut.Now().Should().Be(newFrozenTime);
    }

    [Fact]
    public void FreezeCurrentTime()
    {
        var sut = new TimeTravelersClock();
        sut.Freeze();
        sut.IsFrozen().Should().BeTrue();
        sut.Now().Should().BeCloseTo(DateTimeOffset.Now, _allowedDeviation);
        sut.Now().Should().NotBeAfter(DateTimeOffset.Now);
    }

    [Fact]
    public void ResetFrozenTimeShouldRevertEverythingToNormal()
    {
        var sut = new TimeTravelersClock();
        // arrange
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        // check arrange
        sut.IsFrozen().Should().BeTrue();
        sut.Now().Should().Be(frozenTime);
        // Act
        sut.Reset();
        // Assert
        sut.IsFrozen().Should().BeFalse();
        sut.Now().Should().BeCloseTo(DateTimeOffset.Now, _allowedDeviation);
    }

    [Fact]
    public void BaselineMocksNow()
    {
        var sut = new TimeTravelersClock();
        var now = DateTimeOffset.Now;
        var baseLine = now.AddDays(-1);
        sut.SetNow(baseLine);
        sut.Now().Should().BeCloseTo(baseLine, _allowedDeviation);
        var slept = Sleep(1000);
        sut.Now().Should().BeCloseTo(baseLine.Add(slept), _allowedDeviation);
    }

    [Fact]
    public void AdvanceFrozenTime()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        sut.Now().Should().Be(new DateTimeOffset(2021, 1, 2, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void AdvanceLiveTime()
    {
        var sut = new TimeTravelersClock();
        sut.Advance(TimeSpan.FromDays(1));
        sut.Now().Should().BeCloseTo(DateTimeOffset.Now.AddDays(1), _allowedDeviation);
    }

    [Fact]
    public void AdvanceFrozenTimeAndThenThaw()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        sut.Thaw();
        var slept = Sleep(1000);
        sut.Now().Should().BeCloseTo(frozenTime.AddDays(1).Add(slept), _allowedDeviation);
    }

    [Fact]
    public void AComplicatedTest()
    {
        var sut = new TimeTravelersClock();
        var baseLine = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timer = new Stopwatch(); timer.Start();
        sut.SetNow(baseLine);
        sut.Now().Should().BeCloseTo(baseLine.Add(timer.Elapsed), _allowedDeviation);
        Sleep(20);
        sut.Now().Should().BeCloseTo(baseLine.Add(timer.Elapsed), _allowedDeviation);
        var frozen = sut.Freeze();
        sut.Now().Should().Be(frozen);
        var sleptWhileFrozen = Sleep(2000);
        sut.Thaw();
        Sleep(1000);
        sut.Now().Should().BeCloseTo(baseLine.Add(timer.Elapsed).Add(-sleptWhileFrozen), _allowedDeviation);
    }

    private static TimeSpan Sleep(int milliseconds)
    {
        var before = DateTimeOffset.Now;
        Thread.Sleep(milliseconds);
        var after = DateTimeOffset.Now;
        return after - before;
    }
}