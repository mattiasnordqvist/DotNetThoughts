```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .And(customer => OrderRepository.Query(x => x.CustomerId == customerId)) // Combines the previous result with the new result, and returns a Result with a tuple-value containing both customer and orders.
    .Bind((customer, orders) => customer.SetOrderCount(orders.Count())); 
```

Instead of using And like this, there's another trick we can do in C#.

If implement some variations of SelectMany, ducktyping will give us the ability to write this instead


```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .And(customer => OrderRepository.Query(x => x.CustomerId == customerId)) // Combines the previous result with the new result, and returns a Result with a tuple-value containing both customer and orders.
    .Bind((customer, orders) => customer.SetOrderCount(orders.Count())); 
```
