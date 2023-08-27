using System.Diagnostics.Contracts;

namespace Results;

/// <summary>
/// Extension methods and utilities for working with Unit Results.
/// </summary>
public static class UnitResult
{
    /// <summary>
    /// Converts a Result of type T to a Result of type Unit by throwing away the value, preserving only success or failure, along with errors.
    /// </summary>
    /// <typeparam name="T">The very unimportant type of the value being thrown away</typeparam>
    /// <param name="resultOfType">The result to convert to a Result of Unit</param>
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

    [Pure]
    public static Result<Unit> Error(IEnumerable<IError> errors) => Result<Unit>.Error(errors);
    [Pure]
    public static Result<Unit> Error(IError error, params IError[] errors) => Result<Unit>.Error(errors.Concat(new IError[] { error }));
    [Pure]
    public static Result<Unit> Error(IError error, IEnumerable<IError> errors) => Result<Unit>.Error(errors.Concat(new IError[] { error }));
}
