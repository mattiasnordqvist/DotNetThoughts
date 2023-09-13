using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;

/// <summary>
/// Read on the alternative interpretation https://fsharpforfunandprofit.com/posts/elevated-world/#map
/// </summary>
public static partial class Extensions
{
    #region Result<T> -> (T -> U) -> Result<U>

    [Pure]
    public static Result<U> Map<T, U>(this Result<T> source, Func<T, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, U>(this Result<(T, T2)> source, Func<T, T2, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, U>(this Result<(T, T2, T3)> source, Func<T, T2, T3, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, U>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, U>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, U>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, U>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, T8, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11))
        : Result<U>.Error(source.Errors);

    [Pure]
    public static Result<U> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U> next) =>
        source.Success
        ? Result<U>.Ok(next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11, source.Value.Item12))
        : Result<U>.Error(source.Errors);

    #endregion

    #region Result<T> -> (T -> Task<U>) -> Task<Result<U>>
    [Pure]
    public static async Task<Result<U>> Map<T, U>(this Result<T> source, Func<T, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, U>(this Result<(T, T2)> source, Func<T, T2, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, U>(this Result<(T, T2, T3)> source, Func<T, T2, T3, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, U>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, U>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, U>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, U>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, Task<U>> next) =>
            source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11, source.Value.Item12))
        : await Task.FromResult(Result<U>.Error(source.Errors));

    #endregion

    #region Task<Result<T>> -> (T -> U) -> Task<Result<U>>
    [Pure]
    public static async Task<Result<U>> Map<T, U>(this Task<Result<T>> source, Func<T, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, U>(this Task<Result<(T, T2)>> source, Func<T, T2, U> next) =>
       (await source).Success
       ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2))
       : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, U>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, U> next) =>
      (await source).Success
      ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3))
      : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, U>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, U>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, U>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U> next) =>
            (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11))
        : Result<U>.Error((await source).Errors);

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U> next) =>
        (await source).Success
        ? Result<U>.Ok(next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11, (await source).Value.Item12))
        : Result<U>.Error((await source).Errors);
    #endregion

    #region Task<Result<T>> -> (T -> Task<U>) -> Task<Result<U>>
    [Pure]
    public static async Task<Result<U>> Map<T, U>(this Task<Result<T>> source, Func<T, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, U>(this Task<Result<(T, T2)>> source, Func<T, T2, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, U>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, U>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, U>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, U>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    [Pure]
    public static async Task<Result<U>> Map<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11, (await source).Value.Item12))
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    #endregion

    #region Result<()> -> (() -> U) -> Result<U>
    [Pure]
    public static Result<U> Map<U>(this Result<Unit> source, Func<U> next) =>
        source.Success
        ? Result<U>.Ok(next())
        : Result<U>.Error(source.Errors);

    #endregion

    #region Result<()> -> (() -> Task<U>) -> Task<Result<U>>

    [Pure]
    public static async Task<Result<U>> Map<U>(this Result<Unit> source, Func<Task<U>> next) =>
        source.Success
        ? Result<U>.Ok(await next())
        : await Task.FromResult(Result<U>.Error(source.Errors));

    #endregion

    #region Task<Result<()>> -> (() -> U) -> Task<Result<U>>
    [Pure]
    public static async Task<Result<U>> Map<U>(this Task<Result<Unit>> source, Func<U> next) =>
        (await source).Success
        ? Result<U>.Ok(next())
        : Result<U>.Error((await source).Errors);

    #endregion

    #region Task<Result<()>> -> (() -> Task<U>) -> Task<Result<U>>
    [Pure]
    public static async Task<Result<U>> Map<U>(this Task<Result<Unit>> source, Func<Task<U>> next) =>
        (await source).Success
        ? Result<U>.Ok(await next())
        : await Task.FromResult(Result<U>.Error((await source).Errors));

    #endregion
}
