# Local Time Kit

## LocalDateTime

`LocalDateTime` is an unambiguous representation of an instance in time.
Types like `DateTime` and `DateOnly` does not provide enough information to determine exactly what instance in time they represent. LocalDateTime aims to package all you need into one type.

Don't worry about the word "Local" in the name. LocalDateTime is not limited to local instances in time in the normal sense. LocalDateTime consider even a UTC date to be local. Local to the utc time zone.  

While .Net has a lot of shady "helpful" implicit conversions between `DateTime` and `DateTimeOffset`, LocalDateTime is explicit about what it does.  

No time-operations, makes any sense without knowing which timezone you are operating in. Remember, daylight savings have the effect of some days lasting only 23 hours, while others last 25.  
LocalDateTime therefor requires you to specify a timezone when you create an instance.  
LocalDateTime then helps you to convert between timezones, and to calculate timezone-related specific instances of time, like beginning of year, or midnight of current day etc.
