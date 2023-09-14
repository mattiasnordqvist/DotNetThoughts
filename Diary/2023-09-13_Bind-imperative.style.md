```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .And(customer => OrderRepository.Query(x => x.CustomerId == customerId)) // Combines the previous result with the new result, and returns a Result with a tuple-value containing both customer and orders.
    .Bind((customer, orders) => customer.SetOrderCount(orders.Count())); 
```

Instead of using And like this, there's another trick we can do in C#.

If we implement some variations of SelectMany, ducktyping will give us the ability to write this instead.

```csharp
Result<Unit> result = 
    from customer in CustomerRepository.Load(customerId)
    from orders in OrderRepository.Query(x => x.CustomerId == customerId)
    from result in customer.SetOrderCount(orders.Count())
    select result;
```

Values are then automatically "unwrapped" from their results, as long as the Results are successful.

I think this is a bit like the do-notation in Haskell