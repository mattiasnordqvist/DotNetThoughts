using System.Runtime.CompilerServices;

namespace DotNetThoughts.Results.Validation;

public static class GeneralValidation
{
    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it returns a Result with a MissingArgumentError
    /// </summary>
    public static Result<T> Parse<T, TInput>(TInput? input, Func<TInput, Result<T>> parser, [CallerArgumentExpression("input")] string? argumentExpression = null)
        => MissingArgumentError.IfMissing(input, argumentExpression).Bind(() => parser(input!));

    public static Result<T> Parse<T, TInput, TInput2>(TInput? input, TInput2? input2,
        Func<TInput, TInput2, Result<T>> parser,
        [CallerArgumentExpression("input")] string? argumentExpression = null,
        [CallerArgumentExpression("input2")] string? argumentExpression2 = null)
            where TInput : struct
            where TInput2 : struct
        => Extensions.OrResult(
                MissingArgumentError.IfMissing(input, argumentExpression),
                MissingArgumentError.IfMissing(input, argumentExpression2))
            .Bind((_, _) => parser(input!.Value, input2!.Value));

    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it just return a null-valued Result
    /// </summary>
    public static Result<T?> ParseAllowNull<T, TInput>(TInput? input, Func<TInput, Result<T>> parser)
       where TInput : class
       where T : class
     => input is null
         ? Result<T?>.Ok(null)
         : parser(input)
         .Bind(Result<T?>.Ok);

    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it just return a null-valued Result
    /// </summary>
    public static Result<T?> ParseAllowNull<T, TInput>(TInput? input, Func<TInput, Result<T>> parser)
            where TInput : struct
            where T : class
            => input is null
                ? Result<T?>.Ok(null)
                : parser(input.Value)
                .Bind(Result<T?>.Ok);

    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it just return a null-valued Result
    /// </summary>
    public static Result<T?> ParseAllowNullStruct<T, TInput>(TInput? input, Func<TInput, Result<T>> parser)
       where TInput : class
       where T : struct
     => input is null
         ? Result<T?>.Ok(null)
         : parser(input).Bind(x => Result<T?>.Ok(x));

    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it just return a null-valued Result
    /// </summary>
    public static Result<T?> ParseAllowNullStruct<T, TInput>(TInput? input, Func<TInput, Result<T>> parser)
            where TInput : struct
            where T : struct
            => input is null
                ? Result<T?>.Ok(null)
                : parser(input.Value).Bind(x => Result<T?>.Ok(x));

    /// <summary>
    /// Parses the input using the supplied function, unless it is null, in which case it just return a Result with the given default value
    /// </summary>
    public static Result<T> ParseOrDefaultOnNull<T, TInput>(TInput? input, Func<TInput, Result<T>> parser, T defaultIfNull)
    where T : class => input is null
        ? Result<T>.Ok(defaultIfNull)
        : parser(input);

    /// <summary>
    /// Returns the input if it is not null, wrapped in an ok Result. Otherwise, returns a Result with a MissingArgumentError
    /// </summary>
    public static Result<TInput> Value<TInput>(TInput? input, [CallerArgumentExpression("input")] string? argumentExpression = null) where TInput : struct
        => Parse(input, x => Result<TInput>.Ok(x!.Value), argumentExpression);

    /// <summary>
    /// Returns the input if it is not null, wrapped in an ok Result. Otherwise, returns a Result with a MissingArgumentError
    /// </summary>
    public static Result<TInput> Value<TInput>(TInput? input, [CallerArgumentExpression("input")] string? argumentExpression = null) where TInput : class
        => Parse(input, x => Result<TInput>.Ok(x!), argumentExpression);

    /// <summary>
    /// If the given list or any of its elements is null, returns an error Result with a MissingArgumentError.
    /// Otherwise, parses each element of the list using the given function, and returns an OK result with a
    /// non-null List of parsed elements.
    /// 
    /// Short-circuits on first missing argument.
    /// </summary>
    public static Result<List<T>> ParseEach<T, TInput>(List<TInput?>? input, Func<TInput, Result<T>> parser, [CallerArgumentExpression("input")] string? argumentExpression = null) =>
        Parse(input, elements => elements.Return<IEnumerable<TInput?>>().BindEach(el => Parse(el, parser)), argumentExpression);

    /// <summary>
    /// If the given list or any of its elements is null, returns an error Result with a MissingArgumentError.
    /// Otherwise, parses each element of the list using the given function, and returns an OK result with a
    /// non-null List of parsed elements.
    /// 
    /// Does not short-circuit
    /// </summary>
    public static Result<List<T>> ParseAll<T, TInput>(List<TInput?>? input, Func<TInput, Result<T>> parser, [CallerArgumentExpression("input")] string? argumentExpression = null) =>
      Parse(input, elements => elements.Return<IEnumerable<TInput?>>().BindAll(el => Parse(el, parser)), argumentExpression);
}
