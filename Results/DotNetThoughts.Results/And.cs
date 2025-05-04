using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;

/// <summary>
/// Special versions of Bind that also keeps the Result-source and returns Result of source and next, combined in a tuple.
/// I can't find any name in the functional world for this. To me, it sound a bit like a join or a multiplication.
/// 
/// If bind can be defined as 
///  Result<T> -> (T -> Result<U>) -> Result<U>
/// Then and is defined as
///  Result<T> -> (T -> Result<U>) -> Result<(T,U)> // The T passes through!
///  
/// File is split into region, with one region for each combination of different inputs 
/// Input is 
/// 1. a Result<T> or a Task<Result<T>>
/// 2. A function that takes a T and retuns a Result<T> or a Task<Result<T>>
/// 
/// Inside each region, there are multiple versions of the same method, with T replaced with a Tuple of 2 or more type parameters
/// </summary>
public static partial class Extensions
{

    #region Result<T> -> (T -> Result<U>) -> Result<(T,U)>

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2)> And<T, T2>(this Result<T> source, Func<T, Result<T2>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3)> And<T, T2, T3>(this Result<(T, T2)> source, Func<T, T2, Result<T3>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4)> And<T, T2, T3, T4>(this Result<(T, T2, T3)> source, Func<T, T2, T3, Result<T4>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5)> And<T, T2, T3, T4, T5>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, Result<T5>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6)> And<T, T2, T3, T4, T5, T6>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, Result<T6>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7)> And<T, T2, T3, T4, T5, T6, T7>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, Result<T7>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7, T8)> And<T, T2, T3, T4, T5, T6, T7, T8>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, Result<T8>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> And<T, T2, T3, T4, T5, T6, T7, T8, T9>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Result<T9>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Result<T10>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result<T11>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result<T12>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11, nextValue)));
    #endregion

    #region Result<T> -> (T -> Task<Result<U>>) -> Task<Result<(T,U)>>

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2)>> And<T, T2>(this Result<T> source, Func<T, Task<Result<T2>>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));
    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Result<(T, T2)> source, Func<T, T2, Task<Result<T3>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4)>> And<T, T2, T3, T4>(this Result<(T, T2, T3)> source, Func<T, T2, T3, Task<Result<T4>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5)>> And<T, T2, T3, T4, T5>(this Result<(T, T2, T3, T4)> source, Func<T, T2, T3, T4, Task<Result<T5>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6)>> And<T, T2, T3, T4, T5, T6>(this Result<(T, T2, T3, T4, T5)> source, Func<T, T2, T3, T4, T5, Task<Result<T6>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7)>> And<T, T2, T3, T4, T5, T6, T7>(this Result<(T, T2, T3, T4, T5, T6)> source, Func<T, T2, T3, T4, T5, T6, Task<Result<T7>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> And<T, T2, T3, T4, T5, T6, T7, T8>(this Result<(T, T2, T3, T4, T5, T6, T7)> source, Func<T, T2, T3, T4, T5, T6, T7, Task<Result<T8>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<Result<T9>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<Result<T10>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<Result<T11>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<Result<T12>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11, nextValue)));

    #endregion

    #region Task<Result<T>> -> (T -> Result<U>) -> Task<Result<(T,U>)>

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2)>> And<T, T2>(this Task<Result<T>> source, Func<T, Result<T2>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Task<Result<(T, T2)>> source, Func<T, T2, Result<T3>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4)>> And<T, T2, T3, T4>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, Result<T4>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5)>> And<T, T2, T3, T4, T5>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, Result<T5>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6)>> And<T, T2, T3, T4, T5, T6>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, Result<T6>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7)>> And<T, T2, T3, T4, T5, T6, T7>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, Result<T7>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> And<T, T2, T3, T4, T5, T6, T7, T8>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, Result<T8>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Result<T9>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Result<T10>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Result<T11>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Result<T12>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    #endregion

    #region Task<Result<T>> -> (T -> Task<Result<U>>) -> Task<Result<T,U>>
    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2)>> And<T, T2>(this Task<Result<T>> source, Func<T, Task<Result<T2>>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Task<Result<(T, T2)>> source, Func<T, T2, Task<Result<T3>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4)>> And<T, T2, T3, T4>(this Task<Result<(T, T2, T3)>> source, Func<T, T2, T3, Task<Result<T4>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5)>> And<T, T2, T3, T4, T5>(this Task<Result<(T, T2, T3, T4)>> source, Func<T, T2, T3, T4, Task<Result<T5>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6)>> And<T, T2, T3, T4, T5, T6>(this Task<Result<(T, T2, T3, T4, T5)>> source, Func<T, T2, T3, T4, T5, Task<Result<T6>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7)>> And<T, T2, T3, T4, T5, T6, T7>(this Task<Result<(T, T2, T3, T4, T5, T6)>> source, Func<T, T2, T3, T4, T5, T6, Task<Result<T7>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> And<T, T2, T3, T4, T5, T6, T7, T8>(this Task<Result<(T, T2, T3, T4, T5, T6, T7)>> source, Func<T, T2, T3, T4, T5, T6, T7, Task<Result<T8>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, Task<Result<T9>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, Task<Result<T10>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<Result<T11>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, nextValue)));

    /// <summary>
    /// Sidechains previous results with next function, keeping the previous results in the returned tuple.
    /// </summary>
    [Pure]
    public static Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>> And<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Task<Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>> source, Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<Result<T12>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, sourceValue.Item3, sourceValue.Item4, sourceValue.Item5, sourceValue.Item6, sourceValue.Item7, sourceValue.Item8, sourceValue.Item9, sourceValue.Item10, sourceValue.Item11, nextValue)));
    #endregion
}