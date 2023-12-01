namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Does not short circuit. Returns all errors or one unit
    /// </summary>
    public static Result<Unit> BindAll<T>(this Result<IEnumerable<T>> source, Func<T, Result<Unit>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<Unit>>();
            foreach (var s in source.Value)
            {
                results.Add(next(s));
            }
            return results.Any(x => !x.Success)
                ? UnitResult.Error(results.SelectMany(x => x.Errors))
                : UnitResult.Ok;
        }
        else
        {
            return Result<Unit>.Error(source.Errors);
        }
    }

    public static async Task<Result<Unit>> BindAll<T>(this Result<IEnumerable<T>> source, Func<T, Task<Result<Unit>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<Unit>>();
            foreach (var s in source.Value)
            {
                results.Add(await next(s));
            }
            return results.Any(x => !x.Success)
                ? UnitResult.Error(results.SelectMany(x => x.Errors))
                : UnitResult.Ok;
        }
        else
        {
            return Result<Unit>.Error(source.Errors);
        }
    }

    /// <summary>
    /// Does not short circuit. Returns all errors or all results
    /// </summary>
    public static Result<List<TResult>> BindAll<T, TResult>(this Result<IEnumerable<T>> source, Func<T, Result<TResult>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in source.Value)
            {
                results.Add(next(s));
            }
            return results.Any(x => !x.Success)
               ? Result<List<TResult>>.Error(results.SelectMany(x => x.Errors))
               : Result<List<TResult>>.Ok(results.Select(x => x.Value).ToList());
        }
        else
        {
            return Result<List<TResult>>.Error(source.Errors);
        }
    }

    [Obsolete("Does not take a Result as input. Use one of the regular extensions that takes a result as input. (Do a .Return() on your non-result object first)")]
    public static Result<List<TResult>> BindAll<T, TResult>(this IEnumerable<T> source, Func<T, Result<TResult>> next)
    {
        var results = new List<Result<TResult>>();

        foreach (var s in source)
        {
            results.Add(next(s));
        }
        return results.Any(x => !x.Success)
            ? Result<List<TResult>>.Error(results.SelectMany(x => x.Errors))
            : Result<List<TResult>>.Ok(results.Select(x => x.Value).ToList());
    }

    [Obsolete("Does not take a Result as input. Use one of the regular extensions that takes a result as input. (Do a .Return() on your non-result object first)")]
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this IEnumerable<T> source, Func<T, Task<Result<TResult>>> next)
    {
        var results = new List<Result<TResult>>();

        foreach (var s in source)
        {
            results.Add(await next(s));
        }
        return results.Any(x => !x.Success)
            ? Result<List<TResult>>.Error(results.SelectMany(x => x.Errors))
            : Result<List<TResult>>.Ok(results.Select(x => x.Value).ToList());
    }

    // with tasks
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IEnumerable<T>>> source, Func<T, Task<Result<Unit>>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<Unit>>();
            foreach (var s in (await source).Value)
            {
                results.Add(await next(s));
            }
            return results.Any(x => !x.Success)
                ? UnitResult.Error(results.SelectMany(x => x.Errors))
                : UnitResult.Ok;
        }
        else
        {
            return Result<Unit>.Error((await source).Errors);
        }
    }

    /// <summary>
    /// Does not short circuit. Returns all errors or all results
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IEnumerable<T>>> source, Func<T, Result<TResult>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in (await source).Value)
            {
                results.Add(next(s));
            }
            return results.Any(x => !x.Success)
               ? Result<List<TResult>>.Error(results.SelectMany(x => x.Errors))
               : Result<List<TResult>>.Ok(results.Select(x => x.Value).ToList());
        }
        else
        {
            return Result<List<TResult>>.Error((await source).Errors);
        }
    }

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IEnumerable<T>>> source, Func<T, Task<Result<TResult>>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in (await source).Value)
            {
                results.Add(await next(s));
            }
            return results.Any(x => !x.Success)
               ? Result<List<TResult>>.Error(results.SelectMany(x => x.Errors))
               : Result<List<TResult>>.Ok(results.Select(x => x.Value).ToList());
        }
        else
        {
            return Result<List<TResult>>.Error((await source).Errors);
        }
    }
}
