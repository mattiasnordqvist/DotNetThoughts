# Time Keeping

## SystemTime

`SystemTime.Now()` is a drop-in replacement for `DateTimeOffset.UtcNow` with a couple of extra features.

You can manipulate time using SystemTime.
This is useful for testing, where you want to test how your code behave at different times, or where you want the code to be deterministic.
`SystemTime` works through `AsyncLocal`, so you do not have to inject a `ITimeProvider` wherever you need to be able to mock time.

Some example usages:

```csharp
// Set time to midnight 2024-01-01 (utc I guess) and stop time from moving.
// This means SystemTime.Now() will always return 2024-01-01
SystemTime.Freeze(DateTimeOffset.Parse("2024-01-01"));

// Move time one day forward. Time is still frozen.
// This means SystemTime.Now() will always return 2024-01-02
SystemTime.Advance(TimeSpan.FromDays(1)); 

// Unfreezes time. Time is now 2024-01-02 but time started moving again.
// This means SystemTime.Now() will work as normal, but at an offset to the real-world time.
SystemTime.Thaw();

// Reset time to real world time.
SystemTime.Reset(); 
```


## TimeTravelersClock

TimeTravelersClock is SystemTime but without the AsyncLocal magic. You can instantiate a new TimeTravelsersClock and manipulate it just like SystemTime.