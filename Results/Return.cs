namespace DotNetThoughts.Results;

/// <summary>
/// Extension methods that can lift a T or Task of T into its corresponding Result counterpart.
/// https://fsharpforfunandprofit.com/posts/elevated-world/#return
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Takes any T and wraps it in a successful Result.
    /// https://fsharpforfunandprofit.com/posts/elevated-world/#return
    /// </summary>
    public static Result<T> Return<T>(this T @this) => Result<T>.Ok(@this);

    /// <summary>
    /// Takes any Task of T and wraps it in a successful Result.
    /// https://fsharpforfunandprofit.com/posts/elevated-world/#return
    /// </summary>
    public static async Task<Result<T>> Return<T>(this Task<T> @this) => Result<T>.Ok(await @this);
}
