namespace DotNetThoughts.Results;

/// <summary>
/// Combines multiple independent Results into one. If any of the Results is an Error, the resuling Result will also be an Error.
/// This translates roughly to apply (?)
/// https://fsharpforfunandprofit.com/posts/elevated-world/#apply
/// https://fsharpforfunandprofit.com/posts/elevated-world-3/#dependent
/// </summary>
public static partial class Extensions
{
        public static Result<(T, T2)> OrResult<T, T2>(Result<T> a, Result<T2> b) => a.Or(b);
        public static Result<(T, T2, T3)> OrResult<T, T2, T3>(Result<T> a, Result<T2> b, Result<T3> c) => a.Or(b).Or(c);
        public static Result<(T, T2, T3, T4)> OrResult<T, T2, T3, T4>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d) => a.Or(b).Or(c).Or(d);
        public static Result<(T, T2, T3, T4, T5)> OrResult<T, T2, T3, T4, T5>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e) => a.Or(b).Or(c).Or(d).Or(e);
        public static Result<(T, T2, T3, T4, T5, T6)> OrResult<T, T2, T3, T4, T5, T6>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f) => a.Or(b).Or(c).Or(d).Or(e).Or(f);
        public static Result<(T, T2, T3, T4, T5, T6, T7)> OrResult<T, T2, T3, T4, T5, T6, T7>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g);
        public static Result<(T, T2, T3, T4, T5, T6, T7, T8)> OrResult<T, T2, T3, T4, T5, T6, T7, T8>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g, Result<T8> h) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g).Or(h);
        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> OrResult<T, T2, T3, T4, T5, T6, T7, T8, T9>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g, Result<T8> h, Result<T9> i) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g).Or(h).Or(i);
        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> OrResult<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g, Result<T8> h, Result<T9> i, Result<T10> j) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g).Or(h).Or(i).Or(j);
        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> OrResult<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g, Result<T8> h, Result<T9> i, Result<T10> j, Result<T11> k) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g).Or(h).Or(i).Or(j).Or(k);
        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> OrResult<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Result<T> a, Result<T2> b, Result<T3> c, Result<T4> d, Result<T5> e, Result<T6> f, Result<T7> g, Result<T8> h, Result<T9> i, Result<T10> j, Result<T11> k, Result<T12> l) => a.Or(b).Or(c).Or(d).Or(e).Or(f).Or(g).Or(h).Or(i).Or(j).Or(k).Or(l);

        public static Result<(T, T2)> Or<T, T2>(this Result<T> a, Result<T2> b) => !a.Success || !b.Success
                ? Result<(T, T2)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2)>.Ok((a.Value, b.Value));
        public static async Task<Result<(T, T2)>> Or<T, T2>(this Task<Result<T>> a, Result<T2> b) => !(await a).Success || !b.Success
               ? Result<(T, T2)>.Error((await a).Errors.Concat(b.Errors))
               : Result<(T, T2)>.Ok(((await a).Value, b.Value));
        public static async Task<Result<(T, T2)>> Or<T, T2>(this Task<Result<T>> a, Task<Result<T2>> b) => !(await a).Success || !(await b).Success
                 ? Result<(T, T2)>.Error((await a).Errors.Concat((await b).Errors))
                 : Result<(T, T2)>.Ok(((await a).Value, (await b).Value));

        public static Result<(T, T2, T3)> Or<T, T2, T3>(this Result<(T, T2)> a, Result<T3> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3)>.Ok((a.Value.Item1, a.Value.Item2, b.Value));
        public static async Task<Result<(T, T2, T3)>> Or<T, T2, T3>(this Task<Result<(T, T2)>> a, Task<Result<T3>> b) => !(await a).Success || !(await b).Success
            ? Result<(T, T2, T3)>.Error((await a).Errors.Concat((await b).Errors))
            : Result<(T, T2, T3)>.Ok(((await a).Value.Item1, (await a).Value.Item2, (await b).Value));

        public static Result<(T, T2, T3, T4)> Or<T, T2, T3, T4>(this Result<(T, T2, T3)> a, Result<T4> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, b.Value));

        public static Result<(T, T2, T3, T4, T5)> Or<T, T2, T3, T4, T5>(this Result<(T, T2, T3, T4)> a, Result<T5> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4, T5)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4, T5)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6)> Or<T, T2, T3, T4, T5, T6>(this Result<(T, T2, T3, T4, T5)> a, Result<T6> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4, T5, T6)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4, T5, T6)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7)> Or<T, T2, T3, T4, T5, T6, T7>(this Result<(T, T2, T3, T4, T5, T6)> a, Result<T7> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4, T5, T6, T7)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4, T5, T6, T7)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7, T8)> Or<T, T2, T3, T4, T5, T6, T7, T8>(this Result<(T, T2, T3, T4, T5, T6, T7)> a, Result<T8> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4, T5, T6, T7, T8)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4, T5, T6, T7, T8)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, a.Value.Item7, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> Or<T, T2, T3, T4, T5, T6, T7, T8, T9>(this Result<(T, T2, T3, T4, T5, T6, T7, T8)> a, Result<T9> b) => !a.Success || !b.Success
                ? Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>.Error(a.Errors.Concat(b.Errors))
                : Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, a.Value.Item7, a.Value.Item8, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> Or<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9)> a, Result<T10> b) => !a.Success || !b.Success
            ? Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>.Error(a.Errors.Concat(b.Errors))
            : Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, a.Value.Item7, a.Value.Item8, a.Value.Item9, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> Or<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10)> a, Result<T11> b) => !a.Success || !b.Success
            ? Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>.Error(a.Errors.Concat(b.Errors))
            : Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, a.Value.Item7, a.Value.Item8, a.Value.Item9, a.Value.Item10, b.Value));

        public static Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> Or<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> a, Result<T12> b) => !a.Success || !b.Success
            ? Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>.Error(a.Errors.Concat(b.Errors))
            : Result<(T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)>.Ok((a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4, a.Value.Item5, a.Value.Item6, a.Value.Item7, a.Value.Item8, a.Value.Item9, a.Value.Item10, a.Value.Item11, b.Value));
}
