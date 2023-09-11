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
