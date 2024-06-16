namespace DotNetThoughts.TimeKeeping;

/// <summary>
/// "Mockable" DateTime that you still can use statically using the ambient context anti-pattern. Let's see how it rolls.
/// Replace with an IDateTime if problems arise.
/// </summary>
public class SystemTime
{
    private static AsyncLocal<TimeTravelersClock> _asyncLocalState = new();

    public static TimeTravelersClock Clock
    {
        get
        {
            if (_asyncLocalState.Value == null)
                _asyncLocalState.Value = new TimeTravelersClock();
            return _asyncLocalState.Value;
        }
    }

    public static DateTimeOffset SetBaseline(DateTimeOffset baseLine)
    {
        return Clock.SetBaseline(baseLine);
    }

    public static DateTimeOffset SetOffset(TimeSpan offset)
    {
        return Clock.SetOffset(offset);
    }
    public static DateTimeOffset Advance(TimeSpan timeSpan)
    {
        return Clock.Advance(timeSpan);
    }

    public static DateTimeOffset AdvanceDays(double days)
    {
        return Clock.AdvanceDays(days);
    }

    public static DateTimeOffset UtcNow()
    {
        return Clock.UtcNow();
    }

    public static DateTimeOffset Freeze(DateTimeOffset baseLine)
    {
        return Clock.Freeze(baseLine);
    }

    public static DateTimeOffset Freeze()
    {
        return Clock.Freeze();
    }

    public static bool IsFrozen()
    {
        return Clock.IsFrozen();
    }

    public static DateTimeOffset Reset()
    {
        return Clock.Reset();
    }

    public static DateTimeOffset Thaw()
    {
        return Clock.Thaw();
    }

    public static void From(TimeTravelersClock xSystemTime)
    {
        Clock.From(xSystemTime);
    }

    public static bool IsManipulated()
    {
        return Clock.IsManipulated();
    }
}
