using System.Diagnostics.Contracts;

namespace DotNetThoughts.Results;

/// <summary>
/// Some default implementation of the IError interface.
/// Defaults both Type and Message to the name of the inheriting type.
/// The value of Type is probably good enough, but please provide a better Message by setting it in the constructor.
/// Also defaults GetData() to return a string representation of all property values on the inheriting type.
/// </summary>
public abstract record ErrorBase : IError
{
    /// <summary>
    /// This implicit operator takes an ErrorBase and returns a failed Result of type Unit, with the passed ErrorBase as the only error.
    /// Can't have this for type generics, because that doesnt work with implicit operators, because an implicit cast operator must be defined in the type of either its return value or its argument.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator Result<Unit>(ErrorBase error) => Result<Unit>.Error(error);

    /// <summary>
    /// Type defaults to the name of the inheriting Type.
    /// </summary>
    public ErrorBase(string message)
    {
        Type = GetType().Name;
        Message = message;
    }

    /// <summary>
    /// Type defaults to the name of the inheriting Type.
    /// Message defaults to the name of the inheriting Type.
    /// </summary>
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

    private Dictionary<string, object?> _data = [];

    /// <summary>
    /// In addition to data you add yourself, this also contains all properties on the inheriting type with property name as key and property value as value.
    /// If you add a key to this dictionary, where the inheriting type has a property with the same name as your key, the value of that dictionary entry will be overwritten by the value of the property.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object?> Data { get => GetData(); }

    [Pure]
    public Dictionary<string, object?> GetData()
    {
        var propValues = GetType()
         .GetProperties()
         .Where(p => p.DeclaringType != typeof(IError) && p.DeclaringType != typeof(ErrorBase))
         .ToDictionary(d => d.Name, d => d.GetValue(this));
        foreach (var prop in propValues)
        {
            _data[prop.Key] = prop.Value;
        }
        return _data;
    }
}