# Parsing input

Fluent Validation made me do this. The problem with fluent validation is that all it does is validating your data, but your validated data will still have types that allow invalid values.
When fluent validation validates your nullable decimal to be not null, you will still sit there with a nullable decimal type. What I want, after such validation, is a non-nullable decimal.

## Usage examples


```csharp
decimal? nullableDecimal = 10;

Result<decimal> validDecimal = Parsers.Value(nullableDecimal);

if (validDecimal.IsValid)
{
	Console.WriteLine(validDecimal.Value);
}
else
{
	Console.WriteLine(validDecimal.ErrorMessage);
}
```



```csharp

using static DotNetThoughts.Results.Parsing.Parsers;

static Result<Id<T>> FromInput<T>(string? candidateId) =>
      long.TryParse(candidateId, out var longCandidate)
          ? FromInput<T>(longCandidate)
          : Result<Id<T>>.Error(new InvalidIdError(candidateId));

public Result<Unit> PlaceOrder(string? currency, int? amount, string? paymentMethod, string? orderId) =>
	Extensions.OrResult(
		Parse<Currency>(currency),
		Value(amount),
		ParseAllowNull<PaymentMethod>(paymentMethod),
		Parse(orderId, Id.FromInput<Order>)
	).Bind((validCurrency, validAmount, validPaymentMethod, validOrderId) => 
	{
		... do something with valid and now typed values!
		return Result<Unit>.Ok();
	});

```
