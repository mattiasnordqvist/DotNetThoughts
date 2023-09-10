using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;
public static partial class And
{
    [Pure]
    public static Result<V> SelectMany<U, V, R>(this Result<R> first, Func<R, Result<U>> getSecond, Func<R, U, V> project) 
        => first.Bind(a => getSecond(a).Map(b => project(a, b)));

    [Pure]
    public static Result<U> Select<U, R>(this Result<R> first, Func<R, U> map) 
        => first.Map(map);

    [Pure]
    public static async Task<Result<U>> Select<U, R>(this Task<Result<R>> first, Func<R, U> map) 
        => (await first).Map(map);

    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Task<Result<R>> first, Func<R, Task<Result<U>>> getSecond, Func<R, U, V> project) 
        => await (await first).Bind(async a => (await getSecond(a)).Map(b => project(a, b)));

    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Result<R> first, Func<R, Task<Result<U>>> getSecond, Func<R, U, V> project)
        => await first.Bind(async a => (await getSecond(a)).Map(b => project(a, b)));

    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Task<Result<R>> first, Func<R, Result<U>> getSecond, Func<R, U, V> project) 
        => (await first).Bind(a => getSecond(a).Map(b => project(a, b)));
}
