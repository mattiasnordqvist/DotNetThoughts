namespace DotNetThoughts.Results;

/// <summary>
/// Extension methods to compose Results by chaining them together.
/// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
/// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
/// 
/// This file contains various overloads and versions of bind to take care of many common combinations of inputs and outputs.
/// * Bind from a Result of T
/// * Bind from a UnitResult
/// * Bind from a Task of Result of T
/// * Bind from a Result of a tuple of up to 12 elements
/// * Bind to a Result of T
/// * Bind to a Task of Result of T
/// https://fsharpforfunandprofit.com/posts/elevated-world-2/#bind
/// </summary>
public static partial class Extensions
{
    #region Result<T> -> (T -> Result<U>) -> Result<U>

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, U>(this Result<T> source, Func<T, Result<U>> next) =>
        source.Success
        ? next(source.Value)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, U>(this Result<(T, T2)> source, Func<T, T2, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, U>(this Result<(T, T2, T3)> source, Func<T, T2, T3, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, U>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, U>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, U>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, U>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, T8, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11)
        : Result<U>.Error(source.Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Result<U>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11, source.Value.Item12)
        : Result<U>.Error(source.Errors);

    #endregion


    #region Result<T> -> (T -> Task<Result<U>>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, U>(this Result<T> source, Func<T, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, U>(this Result<(T, T2)> source, Func<T, T2, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, U>(this Result<(T, T2, T3)> source, Func<T, T2, T3, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, U>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, U>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, U>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, U>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11)
        : Task.FromResult(Result<U>.Error(source.Errors));

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<Result<U>>> next) =>
        source.Success
        ? next(source.Value.Item1, source.Value.Item2, source.Value.Item3, source.Value.Item4, source.Value.Item5, source.Value.Item6, source.Value.Item7, source.Value.Item8, source.Value.Item9, source.Value.Item10, source.Value.Item11, source.Value.Item12)
        : Task.FromResult(Result<U>.Error(source.Errors));

    #endregion

    #region Task<Result<T>> -> (T -> Result<U>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> source, Func<T, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, U>(this Task<Result<(T, T2)>> source, Func<T, T2, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, U>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, U>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, U>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, U>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Result<U>> next) =>
        (await source).Success
        ? next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11, (await source).Value.Item12)
        : Result<U>.Error((await source).Errors);

    #endregion

    #region Task<Result<T>> -> (T -> Task<Result<U>>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> source, Func<T, Task<Result<U>>> next) =>
       (await source).Success
       ? await next((await source).Value)
       : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, U>(this Task<Result<(T, T2)>> source, Func<T, T2, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, U>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, U>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, U>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, U>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11)
        : Result<U>.Error((await source).Errors);

    /// <summary>
    /// Applying Bind on a succesful Result of T will unpack T from the Result, and pass it to the provided T -> "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result of T will not execute the provided T -> "Result of U"-function, because there is no value of T to pass on to it. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, U>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<Result<U>>> next) =>
        (await source).Success
        ? await next((await source).Value.Item1, (await source).Value.Item2, (await source).Value.Item3, (await source).Value.Item4, (await source).Value.Item5, (await source).Value.Item6, (await source).Value.Item7, (await source).Value.Item8, (await source).Value.Item9, (await source).Value.Item10, (await source).Value.Item11, (await source).Value.Item12)
        : Result<U>.Error((await source).Errors);

    #endregion

    #region Result<()> -> (() -> Result<U>) -> Result<U>

    /// <summary>
    /// Applying Bind on a succesful Result will execute the "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result will not execute the provided () -> "Result of U"-function. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Result<U> Bind<U>(this Result<Unit> source, Func<Result<U>> next) =>
        source.Success
        ? next()
        : Result<U>.Error(source.Errors);

    #endregion

    #region Result<()> -> (() -> Task<Result<U>>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result will execute the "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result will not execute the provided () -> "Result of U"-function. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static Task<Result<U>> Bind<U>(this Result<Unit> source, Func<Task<Result<U>>> next) =>
        source.Success
        ? next()
        : Task.FromResult(Result<U>.Error(source.Errors));

    #endregion

    #region Task<Result<()>> -> (() -> Result<U>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result will execute the "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result will not execute the provided () -> "Result of U"-function. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<U>(this Task<Result<Unit>> source, Func<Result<U>> next) =>
        (await source).Success
        ? next()
        : Result<U>.Error((await source).Errors);

    #endregion

    #region Task<Result<()>> -> (() -> Task<Result<U>>) -> Task<Result<U>>

    /// <summary>
    /// Applying Bind on a succesful Result will execute the "Result of U"-function, and return the Result of that function.
    /// Applying Bind on an unsuccesful Result will not execute the provided () -> "Result of U"-function. The errors from the previous Result will be retained, but packed into a Result of U
    /// </summary>
    public static async Task<Result<U>> Bind<U>(this Task<Result<Unit>> source, Func<Task<Result<U>>> next) =>
        (await source).Success
        ? await next()
        : Result<U>.Error((await source).Errors);

    #endregion
}