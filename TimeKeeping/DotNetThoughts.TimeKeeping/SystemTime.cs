namespace DotNetThoughts.TimeKeeping;

/// <summary>
/// "Mockable" DateTime that you still can use statically using the ambient context anti-pattern. Let's see how it rolls.
/// </summary>
public class SystemTime
{
    private static AsyncLocal<TimeTravelersClock> _asyncLocalState = new();

    /// <summary>
    /// Returns the underlying <see cref="TimeTravelersClock"/> instance.
    /// </summary>
    public static TimeTravelersClock Clock
    {
        get
        {
            if (_asyncLocalState.Value == null)
                _asyncLocalState.Value = new TimeTravelersClock();
            return _asyncLocalState.Value;
        }
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.SetNow(DateTimeOffset)"/>
    /// </summary>
    /// <param name="baseLine"></param>
    /// <returns></returns>
    public static DateTimeOffset SetNow(DateTimeOffset baseLine)
    {
        return Clock.SetNow(baseLine);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.SetOffset(TimeSpan)"/>
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static DateTimeOffset SetOffset(TimeSpan offset)
    {
        return Clock.SetOffset(offset);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Advance(TimeSpan)"/>
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static DateTimeOffset Advance(TimeSpan timeSpan)
    {
        return Clock.Advance(timeSpan);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.AdvanceDays(double)"/>
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTimeOffset AdvanceDays(double days)
    {
        return Clock.AdvanceDays(days);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Now"/>
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset Now()
    {
        return Clock.Now();
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Freeze(DateTimeOffset)"/>
    /// </summary>
    /// <param name="now"></param>
    /// <returns></returns>
    public static DateTimeOffset Freeze(DateTimeOffset now)
    {
        return Clock.Freeze(now);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Freeze()"/>
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset Freeze()
    {
        return Clock.Freeze();
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.IsFrozen"/>
    /// </summary>
    /// <returns></returns>
    public static bool IsFrozen()
    {
        return Clock.IsFrozen();
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Reset"/>
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset Reset()
    {
        return Clock.Reset();
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.Thaw"/>
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset Thaw()
    {
        return Clock.Thaw();
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.SyncWith(TimeTravelersClock)"/>
    /// </summary>
    /// <param name="otherTimeTravelersClock"></param>
    public static void SyncWith(TimeTravelersClock otherTimeTravelersClock)
    {
        Clock.SyncWith(otherTimeTravelersClock);
    }

    /// <summary>
    /// See <see cref="TimeTravelersClock.IsManipulated"/>
    /// </summary>
    /// <returns></returns>
    public static bool IsManipulated()
    {
        return Clock.IsManipulated();
    }
}
