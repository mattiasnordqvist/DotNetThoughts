namespace DotNetThoughts.TimeKeeping;

/// <summary>
/// A clock that can be manipulated to simulate time travel and time freezing.
/// </summary>
public class TimeTravelersClock
{
    /// <summary>
    /// Don't use this. It is internal stuff. Only read or write as part of serialization.
    /// </summary>
    public DateTimeOffset? FreezedAt { get; set; }

    /// <summary>
    /// Don't use this. It is internal stuff. Only read or write as part of serialization.
    /// </summary>
    public DateTimeOffset? Frozen { get; set; }

    /// <summary>
    /// Don't use this. It is internal stuff. Only read or write as part of serialization.
    /// </summary>
    public TimeSpan Offset { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Returns true if the clock is currently frozen and time is standing still, otherwise false.
    /// When time is frozen, a series of calls to Now() will return the same value.
    /// </summary>
    public bool IsFrozen()
    {
        using var context = new OperationContext();
        return FreezedAt != null;
    }

    /// <summary>
    /// Resets the clock to the current time and removes any time manipulation.
    /// This means the clock is not frozen and time is not offset.
    /// </summary>
    /// <returns>Now()</returns>
    public DateTimeOffset Reset()
    {
        using var context = new OperationContext();
        Frozen = null;
        FreezedAt = null;
        Offset = TimeSpan.Zero;
        return Now();
    }

    /// <summary>
    /// Thaws the clock if it is currently frozen.
    /// </summary>
    /// <returns>Now()</returns>
    /// <exception cref="InvalidOperationException">If you're trying to that a clock that is not frozen.</exception>
    public DateTimeOffset Thaw()
    {
        using var context = new OperationContext();
        if (FreezedAt == null)
        {
            throw new InvalidOperationException("Cannot thaw when not frozen");
        }
        Offset = Offset.Add(-RealTimeFrozen());
        FreezedAt = null;
        Frozen = null;
        return Now();
    }

    /// <summary>
    /// Stops time from moving forward.
    /// A series of calls to Now() will return the same value.
    /// </summary>
    /// <returns>Now()</returns>
    public DateTimeOffset Freeze()
    {
        using var context = new OperationContext();
        FreezedAt = OperationContext.Now();
        Frozen = Now();
        return Frozen.Value;
    }

    /// <summary>
    /// Stops time from moving forward.
    /// A series of calls to Now() will return the passed time `now`.
    /// </summary>
    /// <param name="now">The time the clock supposedly was when frozen.</param>
    /// <returns>Now()</returns>
    public DateTimeOffset Freeze(DateTimeOffset now)
    {
        using var context = new OperationContext();
        Reset();
        SetNow(now);
        FreezedAt = OperationContext.Now();
        Frozen = Now();
        return Frozen.Value;
    }

    /// <summary>
    /// Returns the current time, manipulated or not.
    /// </summary>
    /// <returns>The time of the time travelers clock</returns>
    public DateTimeOffset Now()
    {
        using var context = new OperationContext();
        return Frozen ?? OperationContext.Now().Add(Offset);
    }

    /// <summary>
    /// Shortcut for advancing the clock by a number of days.
    /// Works on both frozen and unfrozen clocks.
    /// </summary>
    /// <param name="days"></param>
    /// <returns>Now()</returns>
    public DateTimeOffset AdvanceDays(double days)
    {
        using var context = new OperationContext();
        return Advance(TimeSpan.FromDays(days));
    }

    /// <summary>
    /// Advances the clock by a given time span.
    /// Works on both frozen and unfrozen clocks.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns>Now()</returns>
    public DateTimeOffset Advance(TimeSpan timeSpan)
    {
        using var context = new OperationContext();
        Offset = Offset.Add(timeSpan);
        if (IsFrozen())
        {
            Frozen = Frozen!.Value.Add(timeSpan);
        }
        return Now();
    }

    /// <summary>
    /// Instead of advancing the clock, this method sets the clock to a specific time.
    /// An unfrozen clock will still be unfrozen, and a frozen clock will still be frozen, but frozen at the given time.
    /// </summary>
    /// <param name="now"></param>
    /// <returns>Now()</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public DateTimeOffset SetNow(DateTimeOffset now)
    {
        if (IsFrozen()) throw new InvalidOperationException();
        using var context = new OperationContext();
        Offset = now - OperationContext.Now();
        return Now();
    }

    /// <summary>
    /// Sets the clocks offset (offset to real world clock) to a specific time span.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Invalid operation on a frozen clock! (I forgot why)</exception>
    public DateTimeOffset SetOffset(TimeSpan offset)
    {
        if (IsFrozen()) throw new InvalidOperationException();
        using var context = new OperationContext();
        Offset = offset;

        return Now();
    }

    private TimeSpan RealTimeFrozen()
    {
        using var context = new OperationContext();
        return FreezedAt != null ? OperationContext.Now() - FreezedAt.Value : TimeSpan.Zero;
    }

    /// <summary>
    /// Syncs this clock with another time travelers clock.
    /// </summary>
    public void SyncWith(TimeTravelersClock otherTimeTravelersClock)
    {
        using var context = new OperationContext();
        Offset = otherTimeTravelersClock.Offset;
        FreezedAt = otherTimeTravelersClock.FreezedAt;
        Frozen = otherTimeTravelersClock.Frozen;
    }

    /// <summary>
    /// Returns true if the clock is currently manipulated in any way. (frozen or offset)
    /// </summary>
    /// <returns></returns>
    public bool IsManipulated()
    {
        return Offset != TimeSpan.Zero || FreezedAt != null;
    }

    internal class OperationContext : IDisposable
    {
        private static readonly AsyncLocal<DateTimeOffset?> _operationNow = new AsyncLocal<DateTimeOffset?>();

        private readonly bool _shouldReset = false;

        public OperationContext()
        {
            if (_operationNow.Value == null)
            {
                _operationNow.Value = DateTimeOffset.UtcNow;
                _shouldReset = true;
            }
        }
        public void Dispose()
        {
            if (_shouldReset)
            {
                _operationNow.Value = null;
            }
        }

        public static DateTimeOffset Now() => _operationNow.Value!.Value;
    }
}