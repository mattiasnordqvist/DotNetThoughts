namespace DotNetThoughts.Results;
public static partial class Experiments
{
    public static Func<Result<A>, Result<B>> Bind<A, B>(this Func<A, Result<B>> f) => a => a.Bind(f);
    public static Func<Result<A>, Result<B>> Map<A, B>(this Func<A, B> f) => a => a.Map(f);
    public static Func<Result<A>, Result<B>> Apply<A, B>(this Result<Func<A, B>> f) => a => f.Apply(a);
    public static Func<Result<A>, Result<B>> MapInTermsOfReturnAndApply<A, B>(this Func<A, B> f) => f.Return().Apply();
}