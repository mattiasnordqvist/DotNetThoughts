# Results.Json

JSON serialization and deserialization support for Result types with proper error handling.

## TL;DR - Quick Start

Serialize and deserialize Result objects to/from JSON with System.Text.Json:

```csharp
// Install the NuGet package
// Install-Package DotNetThoughts.Results.Json

using DotNetThoughts.Results.Json;
using System.Text.Json;

// Configure JSON options
var options = new JsonSerializerOptions();
options.Converters.Add(new JsonConverterFactoryForResultOfT());

// Serialize successful result
var successResult = Result<string>.Ok("Hello World");
string json = JsonSerializer.Serialize(successResult, options);
// Output: {"Success": true, "Value": "Hello World"}

// Serialize error result  
var errorResult = Result<string>.Error("Something went wrong");
string errorJson = JsonSerializer.Serialize(errorResult, options);
// Output: {"Success": false, "Errors": [{"Type": "Error", "Message": "Something went wrong"}]}

// Deserialize back to Result
var deserialized = JsonSerializer.Deserialize<Result<string>>(json, options);
```

**Note**: Error objects are deserialized as generic `DeserializedError` objects since type information is not preserved during serialization.

## Deep Dive

Package contains code to help serialize and deserialize Result objects to and from JSON.
`JsonConverterFactoryForResultOfT` is a `JsonConverterFactory` that can create `JsonConverter` objects for any `Result<T>` object.
Remember, a Result representing one or more Errors contains a list of objects implementing the `IError` interface.
The actual type of an Error object is not included when serialized. This means, that when deserializing that Error again, it can't be deserialized to its original type.
Instead, the Error object is deserialized to a generic `DeserializedError` object.

### JSON Format Examples

A serialized _Successful_ `Result<T>` will look like this:
```json
{
  "Success": true,
  "Value": "Some value"
}
```
A serialized _Successful_ `Result<Unit>` will look like this:
```json
{
  "Success": true
}
```
A serialized _Error_ `Result<T>` or `Result<Unit>` will look like this:
```json
{
  "Success": false,
  "Errors": [
	{
	  "Type": "MyError",
	  "Message": "Some error message"
	  "Data": {
		"Key": "value",
	  }
	}
  ]
}
```