namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Does not short circuit. Returns all errors or one unit
    /// 
    /// T -> (T-> Result<Unit>) -> Result<Unit>
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
    /// <summary>
    /// T -> (T-> Task<Result<Unit>>) -> Task<Result<Unit>>
    /// </summary>
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

    public static Result<Unit> BindAll<T>(this Result<IEnumerable<T>> source, Func<T, int, Result<Unit>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<Unit>>();
            var index = 0;
            foreach (var s in source.Value)
            {
                results.Add(next(s, index));
                index++;
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

   

    public static async Task<Result<Unit>> BindAll<T>(this Result<IEnumerable<T>> source, Func<T, int, Task<Result<Unit>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<Unit>>();
            var index = 0;
            foreach (var s in source.Value)
            {
                results.Add(await next(s, index));
                index++;
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

    public static Result<List<TResult>> BindAll<T, TResult>(this Result<IEnumerable<T>> source, Func<T, int, Result<TResult>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();
            var index = 0;
            foreach (var s in source.Value)
            {
                results.Add(next(s, index));
                index++;
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

    /// <summary>
    /// Does not short circuit. Returns all errors or all results
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<IEnumerable<T>> source, Func<T, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in source.Value)
            {
                results.Add(await next(s));
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<IEnumerable<T>> source, Func<T, int, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();
            var index = 0;
            foreach (var s in source.Value)
            {
                results.Add(await next(s, index));
                index++;
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

    ////// Same as above, but from tasks


    /// <summary>
    /// Does not short circuit. Returns all errors or one unit
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IEnumerable<T>>> source, Func<T, Result<Unit>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<Unit>>();
            foreach (var s in (await source).Value)
            {
                results.Add(next(s));
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

    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IEnumerable<T>>> source, Func<T, int, Result<Unit>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<Unit>>();
            var index = 0;
            foreach (var s in (await source).Value)
            {
                results.Add(next(s, index));
                index++;
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

    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IEnumerable<T>>> source, Func<T, int, Task<Result<Unit>>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<Unit>>();
            var index = 0;
            foreach (var s in (await source).Value)
            {
                results.Add(await next(s, index));
                index++;
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IEnumerable<T>>> source, Func<T, int, Result<TResult>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<TResult>>();
            var index = 0;
            foreach (var s in (await source).Value)
            {
                results.Add(next(s, index));
                index++;
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

    /// <summary>
    /// Does not short circuit. Returns all errors or all results
    /// </summary>
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IEnumerable<T>>> source, Func<T, int, Task<Result<TResult>>> next)
    {
        if ((await source).Success)
        {
            var results = new List<Result<TResult>>();
            var index = 0;
            foreach (var s in (await source).Value)
            {
                results.Add(await next(s, index));
                index++;
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


    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<IReadOnlyList<T>> source, Func<T, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in source.Value)
            {
                results.Add(await next(s));
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<List<T>> source, Func<T, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var results = new List<Result<TResult>>();

            foreach (var s in source.Value)
            {
                results.Add(await next(s));
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<List<T>>> source, Func<T, Task<Result<TResult>>> next)
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
