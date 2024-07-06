namespace DotNetThoughts.TimeKeeping;

public class TimeTravelersClock
{
    public DateTimeOffset? FreezedAt { get; set; }
    public DateTimeOffset? Frozen { get; set; }
    public TimeSpan Offset { get; set; } = TimeSpan.Zero;

    public bool IsFrozen()
    {
        using var context = new OperationContext();
        return FreezedAt != null;
    }

    public DateTimeOffset Reset()
    {
        using var context = new OperationContext();
        Frozen = null;
        FreezedAt = null;
        Offset = TimeSpan.Zero;
        return UtcNow();
    }

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
        return UtcNow();
    }

    public DateTimeOffset Freeze()
    {
        using var context = new OperationContext();
        FreezedAt = context.UtcNow();
        Frozen = UtcNow();
        return Frozen.Value;
    }

    public DateTimeOffset Freeze(DateTimeOffset baseLine)
    {
        using var context = new OperationContext();
        Reset();
        SetBaseline(baseLine);
        FreezedAt = context.UtcNow();
        Frozen = UtcNow();
        return Frozen.Value;
    }

    public DateTimeOffset UtcNow()
    {
        using var context = new OperationContext();
        return Frozen ?? context.UtcNow().Add(Offset);
    }

    public DateTimeOffset AdvanceDays(double days)
    {
        using var context = new OperationContext();
        return Advance(TimeSpan.FromDays(days));
    }

    public DateTimeOffset Advance(TimeSpan timeSpan)
    {
        using var context = new OperationContext();
        Offset = Offset.Add(timeSpan);
        if (IsFrozen())
        {
            Frozen = Frozen!.Value.Add(timeSpan);
        }
        return UtcNow();
    }

    public DateTimeOffset SetBaseline(DateTimeOffset baseLine)
    {
        if (IsFrozen()) throw new InvalidOperationException();
        using var context = new OperationContext();
        Offset = baseLine - context.UtcNow();
        return UtcNow();
    }

    public DateTimeOffset SetOffset(TimeSpan offset)
    {
        if (IsFrozen()) throw new InvalidOperationException();
        using var context = new OperationContext();
        Offset = offset;

        return UtcNow();
    }

    private TimeSpan RealTimeFrozen()
    {
        using var context = new OperationContext();
        return FreezedAt != null ? context.UtcNow() - FreezedAt.Value : TimeSpan.Zero;
    }

    internal void From(TimeTravelersClock xSystemTime)
    {
        using var context = new OperationContext();
        Offset = xSystemTime.Offset;
        FreezedAt = xSystemTime.FreezedAt;
        Frozen = xSystemTime.Frozen;
    }

    internal bool IsManipulated()
    {
        return Offset != TimeSpan.Zero || FreezedAt != null;
    }

    internal class OperationContext : IDisposable
    {
        private static AsyncLocal<DateTimeOffset?> _operationNow = new AsyncLocal<DateTimeOffset?>();

        private bool _shouldReset = false;

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

        public DateTimeOffset UtcNow() => _operationNow.Value!.Value;
    }
}