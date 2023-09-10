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
    public static Result<(T, T2)> And<T, T2>(this Result<T> source, Func<T, Result<T2>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    public static Result<(T, T2, T3)> And<T, T2, T3>(this Result<(T, T2)> source, Func<T, T2, Result<T3>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));
    #endregion

    #region Result<T> -> (T -> Task<Result<U>>) -> Task<Result<(T,U)>>
    public static Task<Result<(T, T2)>> And<T, T2>(this Result<T> source, Func<T, Task<Result<T2>>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Result<(T, T2)> source, Func<T, T2, Task<Result<T3>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));
    #endregion

    #region Task<Result<T>> -> (T -> Result<U>) -> Task<Result<(T,U>)>
    public static Task<Result<(T, T2)>> And<T, T2>(this Task<Result<T>> source, Func<T, Result<T2>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Task<Result<(T, T2)>> source, Func<T, T2, Result<T3>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));
    #endregion

    #region Task<Result<T>> -> (T -> Task<Result<U>>) -> Task<Result<T,U>>
    public static Task<Result<(T, T2)>> And<T, T2>(this Task<Result<T>> source, Func<T, Task<Result<T2>>> next)
        => source.Bind(sourceValue => next(sourceValue).Map(nextValue => (sourceValue, nextValue)));

    public static Task<Result<(T, T2, T3)>> And<T, T2, T3>(this Task<Result<(T, T2)>> source, Func<T, T2, Task<Result<T3>>> next)
        => source.Bind(sourceValue => next(sourceValue.Item1, sourceValue.Item2).Map(nextValue => (sourceValue.Item1, sourceValue.Item2, nextValue)));

    #endregion
}