namespace DotNetThoughts.Results.Validation;

public static class EnumLoader
{
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
}
