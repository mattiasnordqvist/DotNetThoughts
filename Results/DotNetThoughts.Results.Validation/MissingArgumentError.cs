using System.Runtime.CompilerServices;

namespace DotNetThoughts.Results.Validation;

/// <summary>
/// Represents an error that occurs when an argument is missing.
/// </summary>
public record MissingArgumentError : Error
{
    public MissingArgumentError(string? argumentExpression = null)
    {
        if (argumentExpression != null)
            Message = $"Argument '{argumentExpression}' is missing";
    }

    /// <summary>
    /// Returns a <see cref="Result{T}"/> with an <see cref="MissingArgumentError"/> if <paramref name="argument"/> is null.
    /// </summary>
    public static Result<Unit> IfMissing<T>(T? argument, [CallerArgumentExpression("argument")] string? argumentEpression = null) => argument is null ? UnitResult.Error(new MissingArgumentError(argumentEpression)) : UnitResult.Ok;
}
