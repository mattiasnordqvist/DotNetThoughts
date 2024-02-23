
Given 
```csharp
    record DivideByZeroError : ErrorBase;
    Result<int> Divide(int a, int b) => b == 0 ? Result<int>.Error(new DivideByZeroError()) : Result<int>.Ok(a / b);
    Result<int> Multiply(int a, int b) => a * b;        
```

These are all equivalent

```csharp
    var result = Divide(1, 2)
        .Bind(x => Multiply(x, 2)
            .Bind(y => Divide(y, x))
        );

    var result = Divide(1, 2)
        .Bind(x => Multiply(x, 2)
            .Map(y => (x, y)))
        .Bind((x, y) => Divide(y, x)
    );

    var result = Divide(1, 2)
        .And(x => Multiply(x, 2))
        .And((x, y) => Divide(y, x));

    var result = from x in Divide(1, 2)
                 from y in Multiply(x, 2)
                 from z in Divide(y, x)
                 select z;
```
        