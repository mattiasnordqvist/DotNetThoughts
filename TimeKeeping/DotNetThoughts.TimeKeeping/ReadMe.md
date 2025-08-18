# TimeKeeping

Time manipulation utilities for testing, including the ability to freeze, advance, and control time in your applications.

## TL;DR - Quick Start

Replace `DateTimeOffset.UtcNow` with `SystemTime.Now()` to gain control over time in your tests:

```csharp
// Install the NuGet package  
// Install-Package DotNetThoughts.TimeKeeping

using DotNetThoughts.TimeKeeping;

// In your application code, use SystemTime instead of DateTimeOffset
var currentTime = SystemTime.Now(); // Drop-in replacement for DateTimeOffset.UtcNow

// In your tests, control time:

// Freeze time at a specific moment
SystemTime.Freeze(DateTimeOffset.Parse("2024-01-01"));
var time1 = SystemTime.Now(); // Always returns 2024-01-01

// Advance time while keeping it frozen
SystemTime.Advance(TimeSpan.FromDays(1)); 
var time2 = SystemTime.Now(); // Always returns 2024-01-02

// Let time flow again but with an offset
SystemTime.Thaw(); // Time moves normally but offset to 2024-01-02

// Reset to real world time
SystemTime.Reset();
```

Key benefits:
- No dependency injection needed (uses AsyncLocal)
- Deterministic time for tests
- Time travel capabilities for testing edge cases
- Drop-in replacement for DateTimeOffset.UtcNow

## Deep Dive

### SystemTime

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