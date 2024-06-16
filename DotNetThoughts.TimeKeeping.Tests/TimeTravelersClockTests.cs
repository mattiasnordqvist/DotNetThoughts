using FluentAssertions;

using System.Diagnostics;

using Xunit;

namespace DotNetThoughts.TimeKeeping.Tests;
public class TimeTravelersClockTests
{
    [Fact]
    public void FreezeTime()
    {
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var sut = new TimeTravelersClock();
        sut.Freeze(frozenTime);
        sut.IsFrozen().Should().BeTrue();
        sut.UtcNow().Should().Be(frozenTime);
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
        sut.UtcNow().Should().Be(newFrozenTime);
    }

    [Fact]
    public void FreezeCurrentTime()
    {
        var sut = new TimeTravelersClock();
        sut.Freeze();
        sut.IsFrozen().Should().BeTrue();
        sut.UtcNow().Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMilliseconds(5));
        sut.UtcNow().Should().NotBeAfter(DateTimeOffset.Now);
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
        sut.UtcNow().Should().Be(frozenTime);
        // Act
        sut.Reset();
        // Assert
        sut.IsFrozen().Should().BeFalse();
        sut.UtcNow().Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromMilliseconds(5));
    }

    [Fact]
    public void BaselineMocksNow()
    {
        var sut = new TimeTravelersClock();
        var now = DateTimeOffset.Now;
        var baseLine = now.AddDays(-1);
        sut.SetBaseline(baseLine);
        sut.UtcNow().Should().BeCloseTo(baseLine, TimeSpan.FromMilliseconds(5));
        var slept = Sleep(20);
        sut.UtcNow().Should().BeCloseTo(baseLine.Add(slept), TimeSpan.FromMilliseconds(5));
    }

    [Fact]
    public void AdvanceFrozenTime()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        sut.UtcNow().Should().Be(new DateTimeOffset(2021, 1, 2, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void AdvanceLiveTime()
    {
        var sut = new TimeTravelersClock();
        sut.Advance(TimeSpan.FromDays(1));
        sut.UtcNow().Should().BeCloseTo(DateTimeOffset.Now.AddDays(1), TimeSpan.FromMilliseconds(5));
    }

    [Fact]
    public void AdvanceFrozenTimeAndThenThaw()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        sut.Thaw();
        var slept = Sleep(20);
        sut.UtcNow().Should().BeCloseTo(frozenTime.AddDays(1).Add(slept), TimeSpan.FromMilliseconds(5));
    }

    [Fact]
    public void AComplicatedTest()
    {
        var sut = new TimeTravelersClock();
        var baseLine = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timer = new Stopwatch(); timer.Start();
        sut.SetBaseline(baseLine);
        sut.UtcNow().Should().BeCloseTo(baseLine.Add(timer.Elapsed), TimeSpan.FromMilliseconds(5));
        Sleep(20);
        sut.UtcNow().Should().BeCloseTo(baseLine.Add(timer.Elapsed), TimeSpan.FromMilliseconds(5));
        var frozen = sut.Freeze();
        sut.UtcNow().Should().Be(frozen);
        var sleptWhileFrozen = Sleep(200);
        sut.Thaw();
        Sleep(20);
        sut.UtcNow().Should().BeCloseTo(baseLine.Add(timer.Elapsed).Add(-sleptWhileFrozen), TimeSpan.FromMilliseconds(5));
    }

    private static TimeSpan Sleep(int milliseconds)
    {
        var before = DateTimeOffset.Now;
        Thread.Sleep(milliseconds);
        var after = DateTimeOffset.Now;
        return after - before;
    }
}