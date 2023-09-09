using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;

/// <summary>
/// Extension methods and utilities for working with Unit Results.
/// </summary>
public static class UnitResult
{
    /// <summary>
    /// Converts a Result of type T to a Result of type Unit by throwing away the value, preserving only success or failure, along with errors.
    /// </summary>
    /// <typeparam name="T">The very unimportant type of the value being thrown away</typeparam>
    /// <param name="result">The result to convert to a Result of Unit</param>
    /// <returns></returns>
    [Pure]
    public static Result<Unit> ToUnitResult<T>(this Result<T> result) =>
        result.Success
            ? Ok
            : Error(result.Errors);

    /// <summary>
    /// Creates a successful Result of type Unit. 
    /// While there is no practical difference between two succesful Unit Result instances, just like with how there is no practical difference between two Unit instances, 
    /// we can't enforce singleton behavior here because Result is a struct, not a class, and passing it around will create copies.
    /// 
    /// Maybe we should create a ValueResult, just like .Net has ValueTask for cases like this?
    /// </summary>
    [Pure]
    public static Result<Unit> Ok => Result<Unit>.Ok(Unit.Instance);

    /// <summary>
    /// Creates a failed Result of type T, with at least one error.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<Unit> Error(IError error, params IError[] errors) => Result<Unit>.Error(errors.Concat(new IError[] { error }));

    /// <summary>
    /// Creates a failed Result of type T, with at least one error. Zero errors will throw an exception.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<Unit> Error(IEnumerable<IError> errors) => Result<Unit>.Error(errors);

    /// <summary>
    /// Creates a failed Result of type T, with at least one error.
    /// The passed array of errors will be copied to a new list structure. The array will be left untouched. You can't modify the passed array to modify the errors list of the created result.
    /// Is there a way to tell the compiler and consumer that the passed array will be copied, and that the consumer can't modify the errors list of the created result?
    /// </summary>
    [Pure]
    public static Result<Unit> Error(IError error, IEnumerable<IError> errors) => Result<Unit>.Error(errors.Concat(new IError[] { error }));
}
