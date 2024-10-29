# Results.Json

Package contains code to help serialize and deserialize Result objects to and from JSON.
`JsonConverterFactoryForResultOfT` is a `JsonConverterFactory` that can create `JsonConverter` objects for any `Result<T>` object.
Remember, a Result representing one or more Errors contains a list of objects implementing the `IError` interface.
The actual type of an Error object is not included when serialized. This means, that when deserializing that Error again, it can't be deserialized to its original type.
Instead, the Error object is deserialized to a generic `DeserializedError` object.

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