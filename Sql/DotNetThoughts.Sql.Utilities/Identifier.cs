namespace DotNetThoughts.Sql.Utilities;

/// <summary>
/// https://learn.microsoft.com/en-us/sql/relational-databases/databases/database-identifiers?view=sql-server-ver16
/// </summary>
public class Identifier
{

    public string Delimited { get; }
    public string Regular { get; }
    public Identifier(string value)
    {

        if (!value.StartsWith("["))
        {
            value = "[" + value;
        }
        if (!value.EndsWith("]"))
        {
            value += "]";
        }
        Delimited = value;
        Regular = value[1..^1];
    }

    public override string ToString() => Delimited;
    public static implicit operator string(Identifier sysName) => sysName.Delimited;
    public static implicit operator Identifier(string value) => new(value);

}

public static class IdentifierExtensions
{
    public static Identifier ToIdentifier(this string value) => new(value);
}