namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Short circuits on failure
    /// </summary>
    [Obsolete("Does not take a Result as input. Use one of the regular extensions that takes a result as input. (Do a .Return() on your non-result object first)")]
    public static Result<Unit> BindEach<T>(this IEnumerable<T> source, Func<T, Result<Unit>> next)
    {
        foreach (var s in source)
        {
            var result = next(s);
            if (!result.Success) return result;
        }
        return UnitResult.Ok;
    }

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    [Obsolete("Does not take a Result as input. Use one of the regular extensions that takes a result as input. (Do a .Return() on your non-result object first)")]
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
    [Obsolete("Does not take a Result as input. Use one of the regular extensions that takes a result as input. (Do a .Return() on your non-result object first)")]
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

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static Result<Unit> BindEach<T>(this Result<IEnumerable<T>> source, Func<T, Result<Unit>> next)
    {
        if (source.Success)
        {
            foreach (var s in source.Value)
            {
                var result = next(s);
                if (!result.Success) return result;
            }
            return UnitResult.Ok;
        }
        else
        {
            return UnitResult.Error(source.Errors);
        }
    }

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static async Task<Result<Unit>> BindEach<T>(this Result<IEnumerable<T>> source, Func<T, Task<Result<Unit>>> next)
    {
        if (source.Success)
        {
            foreach (var s in source.Value)
            {
                var result = await next(s);
                if (!result.Success) return result;
            }
            return UnitResult.Ok;
        }
        else
        {
            return UnitResult.Error(source.Errors);
        }
    }

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static Result<List<TResult>> BindEach<T, TResult>(this Result<IEnumerable<T>> source, Func<T, Result<TResult>> next)
    {
        if (source.Success)
        {
            var result = new List<TResult>();
            foreach (var s in source.Value)
            {
                var r = next(s);
                if (!r.Success) return Result<List<TResult>>.Error(r.Errors);
                result.Add(r.Value);
            }
            return Result<List<TResult>>.Ok(result);
        }
        else
        {
            return Result<List<TResult>>.Error(source.Errors);
        }
    }
}
