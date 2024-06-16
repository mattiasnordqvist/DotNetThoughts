namespace DotNetThoughts.Results;

public static partial class Extensions
{
    /// <summary>
    /// Widens T of the Result to a nullable T.
    /// 
    /// It's a shame we can't use an implicit conversion to do this.
    /// We can't even implement this on Result<typeparamref name="T"/> itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns>A new re-typed Result with unchanged value</returns>
    public static Result<T?> ToNullableStruct<T>(this Result<T> result) where T : struct
        => result.Bind<T, T?>(x => (T?)x);

    /// <summary>
    /// Widens T of the Result to a nullable T.
    /// 
    /// It's a shame we can't use an implicit conversion to do this.
    /// We can't even implement this on Result<typeparamref name="T"/> itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns>A new re-typed Result with unchanged value</returns>
    public static Result<T?> ToNullable<T>(this Result<T> result) where T : class
        => result.Bind<T, T?>(x => x);
}
