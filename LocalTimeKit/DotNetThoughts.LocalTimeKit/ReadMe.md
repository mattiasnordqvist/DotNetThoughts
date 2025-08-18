# LocalTimeKit

Unambiguous date and time representation that includes timezone information, eliminating confusion around local vs UTC times.

## TL;DR - Quick Start

Work with dates and times that always know their timezone context:

```csharp
// Install the NuGet package
// Install-Package DotNetThoughts.LocalTimeKit

using DotNetThoughts.LocalTimeKit;

// Create a LocalDateTime with explicit timezone
var newYorkTime = new LocalDateTime(
    new DateOnly(2024, 1, 1), 
    new TimeOnly(15, 30), 
    TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
);

// Convert between timezones safely
var utcTime = newYorkTime.ToTimeZone(TimeZoneInfo.Utc);
var londonTime = newYorkTime.ToTimeZone(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));

// Get timezone-aware calculations
var startOfDay = newYorkTime.StartOfDay();
var endOfYear = newYorkTime.EndOfYear();
```

Key benefits:
- Always knows what timezone it represents
- Safe timezone conversions
- Handles daylight saving time correctly
- No ambiguous DateTime values
- Explicit about timezone context

## Deep Dive

### LocalDateTime

`LocalDateTime` is an unambiguous representation of an instance in time.
Types like `DateTime` and `DateOnly` does not provide enough information to determine exactly what instance in time they represent. LocalDateTime aims to package all you need into one type.

Don't worry about the word "Local" in the name. LocalDateTime is not limited to local instances in time in the normal sense. LocalDateTime consider even a UTC date to be local. Local to the utc time zone.  

While .Net has a lot of shady "helpful" implicit conversions between `DateTime` and `DateTimeOffset`, LocalDateTime is explicit about what it does.  

No time-operations, makes any sense without knowing which timezone you are operating in. Remember, daylight savings have the effect of some days lasting only 23 hours, while others last 25.  
LocalDateTime therefor requires you to specify a timezone when you create an instance.  
LocalDateTime then helps you to convert between timezones, and to calculate timezone-related specific instances of time, like beginning of year, or midnight of current day etc.
