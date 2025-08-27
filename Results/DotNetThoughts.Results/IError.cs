namespace DotNetThoughts.Results;

/// <summary>
/// Represents an error in the case of a failed Result.
/// </summary>
public interface IError
{

    /// <summary>
    /// Constant symbol describing this type of Error. Can be used to "code" against on the receiving end.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Human readable english description of the error (can be dynamic I guess), primarily intended
    /// for developer on the receiving end, but can also be used as a fallback message to a user, in case the error is
    /// not handled explicitly in a user interface.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Returns a Dictionary that may contain instance specific data about the error, that can be used to explain or handle the error.
    /// </summary>
    /// <returns>A dictionary of data needed to describe the error in detail.</returns>
    Dictionary<string, object?> Data { get; }
}
