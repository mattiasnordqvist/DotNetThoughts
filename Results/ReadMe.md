Inspired by scott wlaschins talks about railway oriented programming and functional domain modeling, I decided to implement a Result type in c#. 
This post explains a lot: https://fsharpforfunandprofit.com/posts/elevated-world/
I know lang-ext library already has something similar, but I found the library hard to understand and use, so I decided to make an implementation that fits my needs better.

# The Result type

The result type represent the result of an operation that can either succeed or fail. It serves as an alternative to a traditional combination of a return value (success) and an exception (failure). Instead of throwing exceptions on errors, some prefer to return special values to indicate errors, like null, or false, or some other arbitrary type.
The result type allows a standard way of returning errors without having to resort to special values.
The result type can represent a success or a failure. In the case of a success, the result can also contain a Value; the Value that the operation produced.
Analogous with a void method is the Result<Unit> type. It is used to represent operations that do not produce a value, but can still fail.

## When should I throw exceptions and when should I use an Error Result?
Exceptions still have their places. Consider the following example:
```csharp
public static Result<Unit>() AddTopping(Pizza pizza, Topping topping)
{
	if(pizza == null)
		throw new ArgumentNullException(nameof(pizza));
	if (topping == null)
		throw new ArgumentNullException(nameof(topping));
	if(pizza.Toppings.Contains(topping))
		return Result.Error<Unit>("Topping already added");
	pizza.Toppings.Add(topping);
	return Result<Unit>.Ok();
````

The compiler stops a caller from passing a null topping or a null pizza, but only if they have enabled "nullable reference types", so we can't be sure.
If the caller passes a null value as either argument, that's a programming error. The programmer certainly didn't intend to top null with cheese, and the program should crash. The programmer should fix the bug.
However, the compiler can't stop the caller from passing a topping that has already been added to the pizza. This rule has nothing to do with correctness of code, it's a business rule.

## If an operation cant fail, should I return a successful Result or just the value?
I prefer to return the value not wrapped in a Result. It's simpler and more readable.
Returning a Result that can't be in a failed state, might make the caller investigate how it can fail, just to find out that it can't. 
After the caller found out it can't fail, he might remove the error handling code, skipping the Success-check before retrieving the value. The operation might in the future be changed to be able to fail, and then the error handling code is missing, resulting in an exception. Now, this is just as bad as unchecked exceptions.
By marking your operation with the [Pure]-attribute, you indicate that the operation can't have any side-effects. If someone calls your operation without checking the result, roslyn will warn the consumer with a CA1806. However, sometimes your operation does have side-effects, so the [Pure]-attribute is not the perfect fit here.
If the caller absolutely want a Result, he can always wrap the value in a Result.Ok().

## Should I return void, Unit or Result<Unit> from an operation that doesn't return a value?
For the same reason as above, I'd prefer to return Unit over Result<Unit>. But void is even better. Unit is just there to please the type system in the case of a void Result!

## Implementation alternatives

I decided to implement the Result type as a single object, where you have a state that says whether the result is a success or a failure, and a value that contains the value of the operation, if it succeeded, or the error messages, if it failed.
An alternative approach would be to have two different classes, one for success and one for failure. The success class would contain the value, and the failure class would contain the error messages. They would both implement a common interface. This approach would remove the double meaning of a null value in the Value property. (As of now, we must check for existence of errors to determine whether the result is a success or a fail.
I'm not sure which approach is better, maybe I'll try it some day. 

Instead of having the Unit type represent void operations... maybe we could have a additional Result-type without any type-parameters, to represent void operations.

## More ideas

Is it possible to write a Roslyn analyzer that checks that the success property was checked before accessing Value?
Is it possible to write a Roslyn analyzer that checks that a Result return is never left hanging? Even a Result<Unit> should be checked for success or failure, even though it does not contain any interesting value.

# Return and Bind

The Bind method offers a way to chain multiple functions that return a Result<T>.

Instead of checking for a successful result, and if so, unpack the value and pass it to new function, we can use Bind.

You can compare Bind with several already existing C# and .Net.

For example null coalescing, where you chain properties and method calls as long as they don't return null.

```csharp
var customerName = customer?.FirstName;
var customerZip = customer?.Address?.Zip;
```

Or ContinueWith, which executes the next task when the previous is completed, providing the result of the finished task as parameter to next.

```csharp
createUserTask.ContinueWith(customer => SendEmail(customer));
```



```csharp
int a = 1;
Result<B> b = B(a);
Result<D> d;
if(b.Success) {
    Result<C> c = C(b);
    if(c.Success) {
        d = D(c);
        if(d.Success) {
           // do something with d.
        }
        else
        {
            // do something with the errors from D
        }
    }
    else{
        // do something with the errors from C
    }
}
else{
    // do something with the errors from B
}
```

If you are alright with handling the errors from B, C and D in the same way, the above code is the same as

```csharp
int a = 1;
var d = B(a).Bind(b => C(b)).Bind(c => D(c));
if(d.Success) {
    // do something with d.Value
}
else {
    // do something with d.Errors.
    // d will contain the errors from B if B failed, from C if C failed or D, if D failed.
}
```

By the way, no need to create an anonymous function when the receiving function already matches the required the Bind parameter.

```csharp
int a = 1;
var d = B(a).Bind(C).Bind(D);
if(d.Success){
   // ...
}else{
  // ...
}
```

To get rid of the declaration of a, you can elevate it to a successful Result with `Return`

```csharp

var d = 1.Return().Bind(B).Bind(C).Bind(D);
if(d.Success){
   // ...
}else{
  // ...
}
```

Placing each bind on its own row creates a nice readable flow of actions performed on the evolving result.

```csharp

var d = 1.Return()
    .Bind(B)
    .Bind(C)
    .Bind(D);
if(d.Success){
   // ...
}else{
  // ...
}
```

A typical application use-case-implementation could look like this

```csharp

public Result<BlogPostId> PublishBlogPost(DraftId draftId) =>
    _repository.Load(draftId)
        .Bind(x => x.Publish())
        .Bind(blogPost => _unitOfWork
            .Commit()
            .Bind(_ => blogPost.Id));

// other method signature involved
// On DraftRepository
public Result<Draft> Load(DraftId draftId);

// On Draft
public Result<BlogPost> Publish();

// On UnitOfWork
public Result<Unit> Commit();
```

Each of the methods above can fail for various reasons.
Load failed because there's no draft for the given DraftId, published failed because the draft has not been reviewed, so it cannot be published, or Commit failed because of concurrency problems.

# And

Sometimes when binding, you want to reuse the return value from an earlier bind. 
Stupid example:

Load a customer from the database, load a customers orders from the database, and then update the customers ordersCount by looking at how many orders the customer has. This means we need information from two repositories, to be able to update the customer.
UpdateDenormalizedUserCount could look lie this:


```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .Bind(customer => OrderRepository.Query(x => x.CustomerId == customerId))
    .Bind(orders => customer.SetOrderCount(orders.Count())); // won't compile, customer is not available here!!!!
```

To fix this, you can nest the Binds, so that the customer reference is available in the next bind, like this:

```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .Bind(customer => OrderRepository.Query(x => x.CustomerId == customerId)
        .Bind(orders => customer.SetOrderCount(orders.Count()))); // Binds directly on the returned Result of the function inside the "previous" Bind.
```

"And", can do this for you


```csharp
Result<Unit> result = CustomerRepository.Load(customerId)
    .And(customer => OrderRepository.Query(x => x.CustomerId == customerId)) // Combines the previous result with the new result, and returns a Result with a tuple-value containing both customer and orders.
    .Bind((customer, orders) => customer.SetOrderCount(orders.Count())); 
```


# The from syntax

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
        


# The Case for an IResult
So a college of mine wanted to use the Result-class that I had created. I've used it for 2 years by now so I thought it is probably good and stable enough to share. In my own projects, I had until now just copied the code over from one project to another when I wanted to reuse it. Of course, I had to update code in all my projects when I had new ideas or added some documentation or whatever. I decided it was time to package it into a nuget and so I did. 2 days later my college wanted to make changes to it! How is it possible? I've used it basically without changes for two years.

My college is working in a MVC legacy code base, and wanted to introduce usage of the Result-class slowly, one endpoint at a time. At the same time he wanted to introduce some cross cutting behaviour by using action filters. I think he wanted to log every unsuccessful Result that passed through his action filter. So, he started looking at the result provided in the action filter. He had to check the type of the object, which could be a Result<T> or something completely different. If you know generics, you know that closed version can not be checked against using the IsAssignableTo-method. So his wish was that Result<T> should implement generic-free interface, like IResult. Just exposing the Success-property and the list of IErrors would be good enough to him. Meh. Sure, this is not a bigge. It probably has zero side-effects except that _I_ have to code this now and version bump my nuget and ship a new version. It felt strange that I've been using this class for 2 years without issues, and suddenly I had to make changes to accomodate someone elses random use-case. I didn't want to do it, but I also didnt want him to go copy my original code now when there's a nuget!

Searching the internet for a few seconds, we found this code (https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types).

```csharp
public static bool IsAssignableToGenericType(Type givenType, Type genericType)
{
    var interfaceTypes = givenType.GetInterfaces();

    foreach (var it in interfaceTypes)
    {
        if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            return true;
    }

    if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        return true;

    Type baseType = givenType.BaseType;
    if (baseType == null) return false;

    return IsAssignableToGenericType(baseType, genericType);
}
```

This did the trick for my college, and I could go back to do my own stuff without updating the nuget.
This whole situation is exactly why I tend to not package nugets anymore. I don't want to get in a situation where I have to do anything to get someone elses code working. 
Can I design the Result-class differently to avoid situations like this? Was the code above a good enough solution?
