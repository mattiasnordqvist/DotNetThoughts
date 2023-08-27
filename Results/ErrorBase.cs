using System.Diagnostics;

namespace Results;

/// <summary>
/// Some default implementation of the IError interface.
/// Defaults both Type and Message to the name of the inheriting type.
/// The value of Type is probably good enough, but please provide a better Message by setting it in the constructor.
/// Also defaults GetData() to return a string representation of all property values on the inheriting type.
/// </summary>
[DebuggerDisplay("Type = {Type}, Message = {Message}")]
public abstract record ErrorBase : IError
{
    /// <summary>
    /// This implicit operator takes an ErrorBase and returns a failed Result of type Unit, with the passed ErrorBase as the only error.
    /// Can't have this for type generics, because that doesnt work with implicit operators, because an implicit cast operator must be defined in the type of either its return value or its argument.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator Result<Unit>(ErrorBase error) => Result<Unit>.Error(error);
    public ErrorBase()
    {
        Type = GetType().Name;
        Message = GetType().Name;
    }

    public string Type
    {
        protected set;
        get;
    }

    /// <summary>
    /// Is this good, or is it better to define it as a virtual get property?
    /// </summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Scans the inheriting type for all properties and returns them as a dictionary where property name is key, and the property value is the value.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object?> GetData() => GetType()
         .GetProperties()
         .Where(p => p.DeclaringType != typeof(IError) && p.DeclaringType != typeof(ErrorBase))
         .ToDictionary(d => d.Name, d => d.GetValue(this));
}