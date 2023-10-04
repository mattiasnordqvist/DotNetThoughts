namespace DotNetThoughts.Results;
public static partial class Extensions
{
    public static Result<T> Tap<T>(this Result<T> source, Action<T> next)
    {
        if (source.Success)
        {
            next(source.Value);
        }
        return source;
    }

    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> source, Action<T> next)
    {
        if ((await source).Success)
        {
            next((await source).Value);
        }
        return source;
    }
}
