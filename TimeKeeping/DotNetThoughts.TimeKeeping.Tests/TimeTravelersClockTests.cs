using System.Diagnostics;

namespace DotNetThoughts.TimeKeeping.Tests;
public class TimeTravelersClockTests
{
    private static TimeSpan _allowedDeviation = TimeSpan.FromMilliseconds(100);

    [Test]
    public async Task FreezeTime()
    {
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var sut = new TimeTravelersClock();
        sut.Freeze(frozenTime);
        await Assert.That(sut.IsFrozen()).IsTrue();
        await Assert.That(sut.Now()).IsEqualTo(frozenTime);
    }

    [Test]
    public async Task FreezeAgainOverridesCurrentFreeze()
    {
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newFrozenTime = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var sut = new TimeTravelersClock();
        sut.Freeze(frozenTime);
        sut.Freeze(newFrozenTime);
        await Assert.That(sut.IsFrozen()).IsTrue();
        await Assert.That(sut.Now()).IsEqualTo(newFrozenTime);
    }

    [Test]
    public async Task FreezeCurrentTime()
    {
        var sut = new TimeTravelersClock();
        sut.Freeze();
        await Assert.That(sut.IsFrozen()).IsTrue();
        await Assert.That(sut.Now()).IsEqualTo(DateTimeOffset.Now).Within(_allowedDeviation);
        await Assert.That(sut.Now()).IsLessThan(DateTimeOffset.Now);
    }

    [Test]
    public async Task ResetFrozenTimeShouldRevertEverythingToNormal()
    {
        var sut = new TimeTravelersClock();
        // arrange
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        // check arrange
        await Assert.That(sut.IsFrozen()).IsTrue();
        await Assert.That(sut.Now()).IsEqualTo(frozenTime);
        // Act
        sut.Reset();
        // Assert
        await Assert.That(sut.IsFrozen()).IsFalse();
        await Assert.That(sut.Now()).IsEqualTo(DateTimeOffset.Now).Within(_allowedDeviation);
    }

    [Test]
    public async Task BaselineMocksNow()
    {
        var sut = new TimeTravelersClock();
        var now = DateTimeOffset.Now;
        var baseLine = now.AddDays(-1);
        sut.SetNow(baseLine);
        await Assert.That(sut.Now()).IsEqualTo(baseLine).Within(_allowedDeviation);
        var slept = Sleep(1000);
        await Assert.That(sut.Now()).IsEqualTo(baseLine.Add(slept)).Within(_allowedDeviation);
    }

    [Test]
    public async Task AdvanceFrozenTime()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        await Assert.That(sut.Now()).IsEqualTo(new DateTimeOffset(2021, 1, 2, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public async Task AdvanceLiveTime()
    {
        var sut = new TimeTravelersClock();
        sut.Advance(TimeSpan.FromDays(1));
        await Assert.That(sut.Now()).IsEqualTo(DateTimeOffset.Now.AddDays(1)).Within(_allowedDeviation);
    }

    [Test]
    public async Task AdvanceFrozenTimeAndThenThaw()
    {
        var sut = new TimeTravelersClock();
        var frozenTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        sut.Freeze(frozenTime);
        sut.Advance(TimeSpan.FromDays(1));
        sut.Thaw();
        var slept = Sleep(1000);
        await Assert.That(sut.Now()).IsEqualTo(frozenTime.AddDays(1).Add(slept)).Within(_allowedDeviation);
    }

    [Test]
    public async Task AComplicatedTest()
    {
        var sut = new TimeTravelersClock();
        var baseLine = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timer = new Stopwatch(); timer.Start();
        sut.SetNow(baseLine);
        await Assert.That(sut.Now()).IsEqualTo(baseLine.Add(timer.Elapsed)).Within(_allowedDeviation);
        Sleep(20);
        await Assert.That(sut.Now()).IsEqualTo(baseLine.Add(timer.Elapsed)).Within(_allowedDeviation);
        var frozen = sut.Freeze();
        await Assert.That(sut.Now()).IsEqualTo(frozen);
        var sleptWhileFrozen = Sleep(2000);
        sut.Thaw();
        Sleep(1000);
        await Assert.That(sut.Now()).IsEqualTo(baseLine.Add(timer.Elapsed).Add(-sleptWhileFrozen)).Within(_allowedDeviation);
    }

    private static TimeSpan Sleep(int milliseconds)
    {
        var before = DateTimeOffset.Now;
        Thread.Sleep(milliseconds);
        var after = DateTimeOffset.Now;
        return after - before;
    }
}