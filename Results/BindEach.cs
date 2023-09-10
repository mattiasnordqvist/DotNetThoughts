namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static Result<Unit> BindEach<T>(this IEnumerable<T> source, Func<T, Result<Unit>> next)
    {
        foreach (var s in source)
        {
            var result = next(s);
            if (!result.Success) return result;
        }
        return UnitResult.Ok;
    }

    public static async Task<Result<Unit>> BindEach<T>(this IEnumerable<T> source, Func<T, Task<Result<Unit>>> next)
    {
        foreach (var s in source)
        {
            var result = await next(s);
            if (!result.Success) return result;
        }
        return UnitResult.Ok;
    }

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static Result<List<TResult>> BindEach<T, TResult>(this IEnumerable<T> source, Func<T, Result<TResult>> next)
    {
        var result = new List<TResult>();
        foreach (var s in source)
        {
            var r = next(s);
            if (!r.Success) return Result<List<TResult>>.Error(r.Errors);
            result.Add(r.Value);
        }
        return Result<List<TResult>>.Ok(result);
    }
}
