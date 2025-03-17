using System.Runtime.CompilerServices;

namespace DotNetThoughts.Results.Parsing;

public static class Parsers
{
    #region enum
    /// <summary>
    /// Tries to parse <paramref name="candidate"/> to <typeparamref name="T"/> and returns a <see cref="Result{T}"/> with an <see cref="EnumValueMustExistError"/> if <paramref name="candidate"/> is not a valid value of <typeparamref name="T"/>.
    /// Otherwise, returns a <see cref="Result{T}"/> with the parsed value
    /// 
    /// Case-sensitive
    /// </summary>
    public static Result<T> Parse<T>(string? candidate) where T : struct, Enum =>
        Parse<T>(candidate, false);

    /// <summary>
    /// Tries to parse <paramref name="candidate"/> to <typeparamref name="T"/> and returns a <see cref="Result{T}"/> with an <see cref="EnumValueMustExistError"/> if <paramref name="candidate"/> is not a valid value of <typeparamref name="T"/>.
    /// Otherwise, returns a <see cref="Result{T}"/> with the parsed value
    /// </summary>
    public static Result<T> Parse<T>(string? candidate, bool ignoreCase) where T : struct, Enum =>
    Enum.TryParse<T>(candidate, ignoreCase, out var parsed)
        ? Result<T>.Ok(parsed)
        : Result<T>.Error(new EnumValueMustExistError<T>(candidate));

    /// <summary>
    /// Tries to parse <paramref name="candidate"/> to <typeparamref name="T"/> and returns a <see cref="Result{T}"/> with a null value if <paramref name="candidate"/> is not a valid value of <typeparamref name="T"/>.
    /// Otherwise, returns a <see cref="Result{T}"/> with the parsed value.
    /// 
    /// Case-sensitive
    /// </summary>
    public static Result<T?> ParseAllowNull<T>(string? candidate) where T : struct, Enum =>
       ParseAllowNull<T>(candidate, false);

    /// <summary>
    /// Tries to parse <paramref name="candidate"/> to <typeparamref name="T"/> and returns a <see cref="Result{T}"/> with a null value if <paramref name="candidate"/> is not a valid value of <typeparamref name="T"/>.
    /// Otherwise, returns a <see cref="Result{T}"/> with the parsed value.
    /// </summary>
    public static Result<T?> ParseAllowNull<T>(string? candidate, bool ignoreCase) where T : struct, Enum =>
      candidate is null
          ? Result<T?>.Ok(null)
          : Parse<T>(candidate, ignoreCase)
              .Bind(f => Result<T?>.Ok(f));


    #endregion

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
