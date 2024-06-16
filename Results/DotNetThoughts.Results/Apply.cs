namespace DotNetThoughts.Results;

public static partial class Experiments
{
    public static Result<B> Apply<A, B>(this Result<A> a, Result<Func<A, B>> f)
    {
        return a.Success && f.Success
            ? f.Value(a.Value).Return()
            : Result<B>.Error(a.Errors.Concat(f.Errors));
    }

    public static Result<B> Apply<A, B>(this Result<Func<A, B>> f, Result<A> a)
    {
        return a.Success && f.Success
            ? f.Value(a.Value).Return()
            : Result<B>.Error(a.Errors.Concat(f.Errors));
    }
}
