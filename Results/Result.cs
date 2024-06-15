using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace DotNetThoughts.Results;

/// <summary>
/// The result class Represents a result of an operation that can either be successful or not.
/// If successful, it contains a value of type T, otherwise it contains a list of errors.
/// 
/// A struct is used to avoid the overhead of allocating a new object on the heap. 
/// Results are lightweight, having only a couple of references, and are often thrown away quickly in favor of "the next" Result.
/// 
/// A result is immutable, and can't be modified after creation.
/// </summary>
/// <typeparam name="T">If successful, the Result contains a value of type T. Use the Unit type to represent a void operation.</typeparam>
public readonly record struct Result<T>
{
    /// <summary>
    /// This implicit operator takes a value of type T and returns a successful result containing that value.
    /// It is the same as calling Result.Ok(T t) or t.Return().
    /// </summary>
    /// <param name="result">The value to elevate to a successful Result</param>
    [Pure]
    public static implicit operator Result<T>(T result) => Result<T>.Ok(result);

    /// <summary>
    /// This implicit operator takes a value of type T and returns a completed Task containing a successful result containing that value.
    /// It is the same as calling Task.FromResult(resultOfT)
    /// </summary>
    /// <param name="result">The value to elevate to a successful Result and return as a completed Task</param>
    [Pure]
    public static implicit operator Task<Result<T>>(Result<T> result) => Task.FromResult(result);

    /// <summary>
    /// This implicit operator takes a Result of type T and returns a Result of type Unit by throwing away the value, preserving only success or failure, along with errors.
    /// It is the same as calling result.ToUnitResult().
    /// </summary>
    /// <param name="result"></param>
    [Pure]
    public static implicit operator Result<Unit>(Result<T> result) => result.ToUnitResult();

    /// <summary>
    /// Creates a successful Result of type T.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A successful Result of type T, with the passed value as Value</returns>
    [Pure]
    public static Result<T> Ok(T value) => new(value);

    /// <summary>
    /// Creates a failed Result of type T, with at least one error.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<T> Error(IError error, params IError[] errors) => new(errors.Concat([error]));

    /// <summary>
    /// Creates a failed Result of type T, with at least one error. Zero errors will throw an exception.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<T> Error(IEnumerable<IError> errors) => new(errors);

    /// <summary>
    /// Creates a failed Result of type T, with at least one error.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<T> Error(IError error, IEnumerable<IError> errors) => new(errors.Concat([error]));

    /// <summary>
    /// In the end, the only way to create a successful result is through this constructor, by passing a value of type T, which of course can be null.
    /// </summary>
    private Result(T value)
    {
        _value = value;
        Errors = Array.Empty<IError>(); // This is a successful result, so there are no errors. Reference a cached empty array to avoid allocating a new one for each successful result.
    }

    private Result(IEnumerable<IError> errors)
    {
        // It is semantically incorrect to create a failed result without any errors, so throw an exception if the passed errors list is empty.
        if (!errors.Any()) throw new InvalidOperationException("Can't create an error result without any errors!");
        // Copy the passed errors list to a new list structure. The passed list will be left untouched. This ensures the errors list can't be modified from outside the Result.
        Errors = errors.ToList();
    }

    /// <summary>
    /// Not sure whether this method is good enough to be included in the Result class.
    /// However, it is sometimes useful to have a method that returns the first error of a specific type, if any.
    /// Maybe we should return all errors of the type?
    /// </summary>
    /// <typeparam name="TError">The error to check for</typeparam>
    /// <returns>The error, if a match was found, otherwise null</returns>
    [Pure]
    [Obsolete("Use HasError<TError> instead and use out parameter to capture error instance")]
    public TError? IsError<TError>() where TError : IError => Success
            ? default
            : Errors.Any(x => x.GetType() == typeof(TError))
                ? (TError)Errors.First(x => x.GetType() == typeof(TError))
                : default;

    /// <summary>
    /// Not sure whether this method is good enough to be included in the Result class.
    /// Works much as the other IsError method, but returns a boolean indicating whether a match was found, and an out parameter containing the error, if any.
    /// </summary>
    [Pure]
    [Obsolete("Use HasError<TError> instead")]
    public readonly bool IsError<TError>(out TError? error) where TError : IError
    {
        if (Success)
        {
            error = default;
            return false;

        }
        else if (Errors.Any(x => x.GetType() == typeof(TError)))
        {
            error = (TError)Errors.First(x => x.GetType() == typeof(TError));
            return true;
        }
        else
        {
            error = default;
            return false;
        }
    }

    /// <summary>
    /// Returns the Value of the Result, if it is successful, otherwise throws an exception.
    /// The exception is here to help you find out when you're expectations are wrong! Consider getting this exception as a hint that you should check the Success property before accessing the Value.
    /// Maybe you though the operation could fail, but it could! Maybe whether it can fail or not has changed since you first wrote the operation, because other downstream operations now can fail.
    /// Make a habit of always checking the Success property before accessing the Value, and you'll never get this exception.
    /// </summary>
    [Pure]
    public readonly T Value => Success
        ? _value!
        : throw new InvalidOperationException("Can't get a value from an unsuccesful result.");

    // Holds the value of a successful result. Can be null, if T is a nullable type, or if the result is a failure. 
    // We can't use a value of null alone to determine whether the result is a success or failure, because T can be a nullable type, and null is a valid value for a nullable type.
    private readonly T? _value;

    /// <summary>
    /// Returns whether the result is a success or failure.
    /// </summary>
    [Pure]
    public readonly bool Success => !Errors.Any(); // A successful result has no errors. We check the number of errors instead of checking whether the Value is null, because T can be a nullable type, and null is a valid value for a successful result of the nullable type.

    /// <summary>
    /// A readonly list of errors. Will be empty if the result is a success, but you can safely enumerate it without checking the Success property first.
    /// </summary>
    [Pure]
    public readonly IReadOnlyList<IError> Errors { get; }


    /// <summary>
    /// Call this when you explicitly want a failed Result to throw an exception!
    /// The exception will hold the list of errors, so you can inspect them.
    /// </summary>
    /// <param name="motivation">An optional motivation. Why is it ok for this to throw exception, or why do you expect that it never will?</param>
    /// <returns>The Value, if successful.</returns>
    /// <exception cref="ValueOrThrowException"></exception>

    public T ValueOrThrow(string? motivation = null)
    {
        if (Success) return Value;
        else throw new ValueOrThrowException(Errors, motivation);
    }

    /// <summary>
    /// Returns true if the result contains any error of the given type
    /// The out parameter contains the first instance of such error if return is true.
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <param name="error"></param>
    /// <returns></returns>
    [Pure]
    public bool HasError<TError>([NotNullWhen(true)] out TError? error) where TError : IError
    {
        var firstTError = Errors.FirstOrDefault(x => x.GetType() == typeof(TError));
        if (firstTError is TError { } actualError)
        {
            error = actualError;
            return true;
        }
        else
        {
            error = default;
            return false;
        }
    }

    /// <summary>
    /// Returns true if the result contains any error of the given type
    /// </summary>
    [Pure]
    public bool HasError<TError>() where TError : IError
        => HasError<TError>(out _);

    /// <summary>
    /// Returns true if result contains a single error, and this error is of the given type
    /// The out parameter contains this single instance if return is true.
    /// </summary>
    [Pure]
    public bool HasSingleError<TError>([NotNullWhen(true)] out TError? error) where TError : IError
    {
        if (HasError<TError>(out var actualError) && Errors.Count == 1)
        {
            error = actualError;
            return true;
        }
        else
        {
            error = default;
            return false;
        }
    }

    /// <summary>
    /// /// Returns true if result contains a single error, and this error is of the given type
    /// </summary>
    [Pure]
    public bool HasSingleError<TError>() where TError : IError
        => HasSingleError<TError>(out _);

    /// <summary>
    /// Returns new re-typed result, if the result is an Error. Otherwise throws an exception.
    /// </summary>
    /// <typeparam name="R">Desired Result-type</typeparam>
    /// <returns>Errors from this Result contained in a new Result with the desired type</returns>
    /// <exception cref="InvalidOperationException">If this Result is a success</exception>
    public Result<R> ErrorTo<R>()
    {
        if (Success) throw new InvalidOperationException("Can't convert a successful result to a failed result of another type.");
        return Result<R>.Error(Errors);
    }

    /// <summary>
    /// Override default record print behavior in order to avoid touching <see cref="Value"/> when <see cref="Success"/> is false
    /// </summary>
    bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Success));
        builder.Append(" = ");
        builder.Append(Success);

        if (Success)
        {
            builder.Append(", ");
            builder.Append(nameof(Value));
            builder.Append(" = ");
            builder.Append(Value);
        }
        else
        {
            builder.Append(", ");
            builder.Append(nameof(Errors));
            builder.Append(" = ");
            builder.Append("[ ");
            builder.Append(string.Join(", ", Errors));
            builder.Append(" ]");
        }

        return true;
    }
}

/// <summary>
/// An exception created when a Result contains at least one Error, and ValueOrThrow is invoked on it.
/// </summary>
[Serializable]
public class ValueOrThrowException : Exception
{

    internal ValueOrThrowException(IReadOnlyList<IError> errors, string? motivation)
    {
        Errors = errors;
        Motivation = motivation;
    }

    /// <inheritdoc/>

    [Pure]
    public override string Message
    {
        get
        {
            var message = $"ValueOrThrow was called on a non-successful Result-object. {Environment.NewLine}";
            if (Motivation is not null)
            {
                message += $"The developer provided the following motivation for this ValueOrThrow-invocation: {Environment.NewLine}";
                message += $"- {Motivation}{Environment.NewLine}";
            }
            message += $"{Environment.NewLine}The result contains the following errors: {string.Join("", Errors.Select(x => $"{Environment.NewLine}- {x}"))}";

            return message;
        }
    }

    /// <summary>
    /// The list of errors that were deemed to cause an exception.
    /// </summary>
    public IReadOnlyList<IError> Errors { [Pure] get; private set; }

    /// <summary>
    /// The motivation for the call to the ValueOrThrow that created this Exception, if provided.
    /// </summary>
    public string? Motivation { get; }
}
