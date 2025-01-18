using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Text;

namespace DotNetThoughts.Results;

/// <summary>
/// Some default implementation of the IError interface.
/// Defaults both Type and Message to the name of the inheriting type.
/// The value of Type is probably good enough, but please provide a better Message by setting it in the constructor.
/// Also defaults GetData() to return a string representation of all property values on the inheriting type.
/// </summary>
public abstract record ErrorBase : IError
{
    private static readonly ConcurrentDictionary<Type, string> _cache = new();

    public static string ExpandTypeName(Type t)
    {
        // Check if the result is already cached
        if (_cache.TryGetValue(t, out var cachedResult))
        {
            return cachedResult;
        }

        // Perform the expansion
        string result;

        if (t.IsGenericTypeDefinition)
        {
            var removedBackTick = (t.Name.IndexOf('`') >= 0 ? t.Name.Remove(t.Name.IndexOf('`')) : t.Name);
            var nrOfGenericArguments = t.GetGenericArguments().Length;
            result = removedBackTick + "<" +new string(',',nrOfGenericArguments-1)+ ">";
        }
        else if(!t.IsGenericType)
        {
            result = t.Name;
        }
        else
        {
            var removedBackTick = (t.Name.IndexOf('`') >= 0 ? t.Name.Remove(t.Name.IndexOf('`')) : t.Name);
            result = $"{removedBackTick}<{string.Join(',', t.GetGenericArguments().Select(x => ExpandTypeName(x)))}>";
        }

        // Cache the result before returning
        _cache[t] = result;

        return result;
    }

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
        Type = ExpandTypeName(GetType());
        Message = message;
    }

    /// <summary>
    /// Type defaults to the name of the inheriting Type.
    /// Message defaults to the name of the inheriting Type.
    /// </summary>
    public ErrorBase()
    {
        Type = ExpandTypeName(GetType());
        Message = ExpandTypeName(GetType());
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

    /// <summary>
    /// Override default record print behavior in order print <see cref="Data"/> in a readable way
    /// </summary>
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(Type));
        builder.Append(" = ");
        builder.Append(Type);

        builder.Append(", ");

        builder.Append(nameof(Message));
        builder.Append(" = ");
        builder.Append(Message);

        builder.Append(", ");
        builder.Append(nameof(Data));
        builder.Append(" = ");
        builder.Append("{");
        builder.Append(Data.Any () ? $" {string.Join(", ", Data.Select(x => $"{x.Key} = {x.Value}"))} ": " ");
        builder.Append("}");

        return true;
    }
}