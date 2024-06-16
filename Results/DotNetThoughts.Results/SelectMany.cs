﻿using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;
public static partial class Extensions
{
    /// <summary>
    /// Select has the same type signature as Map. This code is provided for LINQ syntax.
    /// </summary>
    [Pure]
    public static Result<U> Select<U, R>(this Result<R> first, Func<R, U> map)
        => first.Map(map);

    /// <summary>
    /// Select has the same type signature as Map. This code is provided for LINQ syntax.
    /// </summary>
    [Pure]
    public static async Task<Result<U>> Select<U, R>(this Task<Result<R>> first, Func<R, U> map)
        => (await first).Map(map);

    /// <summary>
    /// Select has the same type signature as Map. This code is provided for LINQ syntax.
    /// </summary>
    [Pure]
    public static async Task<Result<U>> Select<U, R>(this Result<R> first, Func<R, Task<U>> map)
        => await (first).Map(map);

    /// <summary>
    /// Select has the same type signature as Map. This code is provided for LINQ syntax.
    /// </summary>
    [Pure]
    public static async Task<Result<U>> Select<U, R>(this Task<Result<R>> first, Func<R, Task<U>> map)
        => await (await first).Map(map);

    /// <summary>
    /// SelectMany has the same type signature as And. This code is provided for LINQ syntax
    /// </summary>
    [Pure]
    public static Result<V> SelectMany<U, V, R>(this Result<R> first, Func<R, Result<U>> getSecond, Func<R, U, V> project) 
        => first.Bind(a => getSecond(a).Map(b => project(a, b)));
    
    /// <summary>
    /// SelectMany has the same type signature as And. This code is provided for LINQ syntax
    /// </summary>
    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Result<R> first, Func<R, Task<Result<U>>> getSecond, Func<R, U, V> project)
        => await first.Bind(async a => (await getSecond(a)).Map(b => project(a, b)));

    /// <summary>
    /// SelectMany has the same type signature as And. This code is provided for LINQ syntax
    /// </summary>
    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Task<Result<R>> first, Func<R, Result<U>> getSecond, Func<R, U, V> project) 
        => (await first).Bind(a => getSecond(a).Map(b => project(a, b)));

    /// <summary>
    /// SelectMany has the same type signature as And. This code is provided for LINQ syntax
    /// </summary>
    [Pure]
    public static async Task<Result<V>> SelectMany<U, V, R>(this Task<Result<R>> first, Func<R, Task<Result<U>>> getSecond, Func<R, U, V> project)
        => await (await first).Bind(async a => (await getSecond(a)).Map(b => project(a, b)));
}
