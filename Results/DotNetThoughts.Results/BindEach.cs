namespace DotNetThoughts.Results;
public static partial class Extensions
{
    #region input-ienumerable
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

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static async Task<Result<List<TResult>>> BindEach<T, TResult>(this Result<IEnumerable<T>> source, Func<T, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var result = new List<TResult>();
            foreach (var s in source.Value)
            {
                var r = await next(s);
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

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static async Task<Result<List<TResult>>> BindEach<T, TResult>(this Task<Result<IEnumerable<T>>> source, Func<T, Task<Result<TResult>>> next)
    {
        if ((await source).Success)
        {
            var result = new List<TResult>();
            foreach (var s in (await source).Value)
            {
                var r = await next(s);
                if (!r.Success) return Result<List<TResult>>.Error(r.Errors);
                result.Add(r.Value);
            }
            return Result<List<TResult>>.Ok(result);
        }
        else
        {
            return Result<List<TResult>>.Error((await source).Errors);
        }
    }
    #endregion

    #region input-list
    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static Result<Unit> BindEach<T>(this Result<List<T>> source, Func<T, Result<Unit>> next)
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
    public static async Task<Result<Unit>> BindEach<T>(this Result<List<T>> source, Func<T, Task<Result<Unit>>> next)
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
    public static Result<List<TResult>> BindEach<T, TResult>(this Result<List<T>> source, Func<T, Result<TResult>> next)
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

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static async Task<Result<List<TResult>>> BindEach<T, TResult>(this Result<List<T>> source, Func<T, Task<Result<TResult>>> next)
    {
        if (source.Success)
        {
            var result = new List<TResult>();
            foreach (var s in source.Value)
            {
                var r = await next(s);
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

    /// <summary>
    /// Short circuits on failure
    /// </summary>
    public static async Task<Result<List<TResult>>> BindEach<T, TResult>(this Task<Result<List<T>>> source, Func<T, Task<Result<TResult>>> next)
    {
        if ((await source).Success)
        {
            var result = new List<TResult>();
            foreach (var s in (await source).Value)
            {
                var r = await next(s);
                if (!r.Success) return Result<List<TResult>>.Error(r.Errors);
                result.Add(r.Value);
            }
            return Result<List<TResult>>.Ok(result);
        }
        else
        {
            return Result<List<TResult>>.Error((await source).Errors);
        }
    }
    #endregion
}
