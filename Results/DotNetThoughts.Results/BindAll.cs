namespace DotNetThoughts.Results;

//# Source
//Result<IEnumerable<T>>
//Task<Result<IEnumerable<T>>>
//Result<List<T>>
//Task<Result<List<T>>>

//# Next x Return
//1. Func<T, Result<Unit>> x Result<Unit>
//2. Func<T, Result<T>> x Result<List<T>>
//3. Func<T, Task<Result<Unit>>> x Task<Result<Unit>>
//4. Func<T, Task<Result<T>>> x Task<Result<List<T>>>

//5. Func<T, int, Result<Unit>> x Result<Unit>
//6. Func<T, int, Result<T>> x Result<List<T>>
//7. Func<T, int, Task<Result<Unit>>> x Task<Result<Unit>>
//8. Func<T, int, Task<Result<T>>> x Task<Result<List<T>>>
public static partial class Extensions
{
    #region source = Result<IEnumerable<T>>

    /// <summary>
    /// 1
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
    /// 2
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

    /// <summary>
    /// 3
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

    /// <summary>
    /// 4
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

    /// <summary>
    /// 5
    /// </summary>
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

    /// <summary>
    /// 6
    /// </summary>
    public static Result<List<TResult>> BindAll<T,TResult>(this Result<IEnumerable<T>> source, Func<T, int, Result<TResult>> next)
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
    /// 7
    /// </summary>
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
    /// 8
    /// </summary>
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

    #endregion

    #region source = Task<Result<IEnumerable<T>>>

    /// <summary>
    /// 1
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

    /// <summary>
    /// 2
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

    /// <summary>
    /// 3
    /// </summary>
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
    /// 4
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

    /// <summary>
    /// 5   
    /// </summary>
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

    /// <summary>
    /// 6
    /// </summary>
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
    /// 7
    /// </summary>
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
    /// 8
    /// </summary>
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

    #endregion

    #region source = Result<List<T>>

    public static Result<Unit> BindAll<T>(this Result<List<T>> source, Func<T, Result<Unit>> next)
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

    public static Result<List<TResult>> BindAll<T, TResult>(this Result<List<T>> source, Func<T, Result<TResult>> next)
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

    public static async Task<Result<Unit>> BindAll<T>(this Result<List<T>> source, Func<T, Task<Result<Unit>>> next)
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

    public static Result<Unit> BindAll<T>(this Result<List<T>> source, Func<T, int, Result<Unit>> next)
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

    public static Result<List<TResult>> BindAll<T, TResult>(this Result<List<T>> source, Func<T, int, Result<TResult>> next)
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

    public static async Task<Result<Unit>> BindAll<T>(this Result<List<T>> source, Func<T, int, Task<Result<Unit>>> next)
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<List<T>> source, Func<T, int, Task<Result<TResult>>> next)
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

    #endregion

    #region source = Task<Result<List<T>>>

    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<List<T>>> source, Func<T, Result<Unit>> next)
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

    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<List<T>>> source, Func<T, Result<TResult>> next)
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
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<List<T>>> source, Func<T, Task<Result<Unit>>> next)
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
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<List<T>>> source, Func<T, int, Result<Unit>> next)
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
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<List<T>>> source, Func<T, int, Result<TResult>> next)
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
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<List<T>>> source, Func<T, int, Task<Result<Unit>>> next)
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
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<List<T>>> source, Func<T, int, Task<Result<TResult>>> next)
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

    #endregion

    #region source = Result<IReadOnlyList<T>>

    /// <summary>
    /// 1
    /// </summary>
    public static Result<Unit> BindAll<T>(this Result<IReadOnlyList<T>> source, Func<T, Result<Unit>> next)
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
    /// 2
    /// </summary>
    public static Result<List<TResult>> BindAll<T, TResult>(this Result<IReadOnlyList<T>> source, Func<T, Result<TResult>> next)
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

    /// <summary>
    /// 3
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Result<IReadOnlyList<T>> source, Func<T, Task<Result<Unit>>> next)
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
    /// 4
    /// </summary>
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

    /// <summary>
    /// 5
    /// </summary>
    public static Result<Unit> BindAll<T>(this Result<IReadOnlyList<T>> source, Func<T, int, Result<Unit>> next)
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

    /// <summary>
    /// 6
    /// </summary>
    public static Result<List<TResult>> BindAll<T, TResult>(this Result<IReadOnlyList<T>> source, Func<T, int, Result<TResult>> next)
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
    /// 7
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Result<IReadOnlyList<T>> source, Func<T, int, Task<Result<Unit>>> next)
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
    /// 8
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Result<IReadOnlyList<T>> source, Func<T, int, Task<Result<TResult>>> next)
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

    #endregion

    #region source = Task<Result<IReadOnlyList<T>>>

    /// <summary>
    /// 1
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IReadOnlyList<T>>> source, Func<T, Result<Unit>> next)
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

    /// <summary>
    /// 2
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IReadOnlyList<T>>> source, Func<T, Result<TResult>> next)
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

    /// <summary>
    /// 3
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IReadOnlyList<T>>> source, Func<T, Task<Result<Unit>>> next)
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
    /// 4
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IReadOnlyList<T>>> source, Func<T, Task<Result<TResult>>> next)
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

    /// <summary>
    /// 5   
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IReadOnlyList<T>>> source, Func<T, int, Result<Unit>> next)
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

    /// <summary>
    /// 6
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IReadOnlyList<T>>> source, Func<T, int, Result<TResult>> next)
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
    /// 7
    /// </summary>
    public static async Task<Result<Unit>> BindAll<T>(this Task<Result<IReadOnlyList<T>>> source, Func<T, int, Task<Result<Unit>>> next)
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
    /// 8
    /// </summary>
    public static async Task<Result<List<TResult>>> BindAll<T, TResult>(this Task<Result<IReadOnlyList<T>>> source, Func<T, int, Task<Result<TResult>>> next)
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

    #endregion

}
