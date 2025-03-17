using DotNetThoughts.Results;

namespace DotNetThoughts.Results.Parsing;

/// <summary>
/// Represents an error that occurs when the value of a string cannot be parsed to a given enum.    
/// </summary>
public record EnumValueMustExistError<T> : Error where T : struct, Enum
{
    public string EnumName => typeof(T).Name;
    public string[] ValidValues => Enum.GetValues<T>().Select(x => x.ToString()).ToArray();
    public EnumValueMustExistError(string? candidate)
    {

        Message = (candidate?.ToString() ?? "<null>") + $" is not a valid {EnumName}. Valid {EnumName} alternatives: " +
            string.Join(", ", ValidValues);
    }
}
