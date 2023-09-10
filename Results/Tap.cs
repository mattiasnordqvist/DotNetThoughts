namespace DotNetThoughts.Results;
public static partial class And
{
    public static Result<Unit> Tap<T>(this Result<T> source, Action<T> next)
    {
        if (source.Success)
        {
            next(source.Value);
            return UnitResult.Ok;
        }
        return UnitResult.Error(source.Errors);
    }

    public static async Task<Result<Unit>> Tap<T>(this Task<Result<T>> source, Action<T> next)
    {
        if ((await source).Success)
        {
            next((await source).Value);
            return UnitResult.Ok;
        }
        return UnitResult.Error((await source).Errors);
    }

}
