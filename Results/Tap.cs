namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Tap into the result and perform an action if the result is successful.
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> source, Action<T> next)
    {
        if (source.Success)
        {
            next(source.Value);
        }
        return source;
    }

    /// <summary>
    /// Tap into the result and perform an action if the result is successful.
    /// </summary>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> source, Action<T> next)
    {
        if ((await source).Success)
        {
            next((await source).Value);
        }
        return await source;
    }

    /// <summary>
    /// Tap into the result and perform an action if the result is successful.
    /// </summary>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> source, Func<T, Task> next)
    {
        if ((await source).Success)
        {
            await next((await source).Value);
        }
        return await source;
    }

    /// <summary>
    /// Tap into the result and perform an action if the result is successful.
    /// </summary>
    public static async Task<Result<T>> Tap<T>(this Result<T> source, Func<T, Task> next)
    {
        if (source.Success)
        {
            await next(source.Value);
        }
        return source;
    }
}
