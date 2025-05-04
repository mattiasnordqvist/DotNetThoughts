using System.Text;
namespace DotNetThoughts.Sql.Migrations;

public static class Utilities
{
    public static IEnumerable<string> BatchSplit(string sql)
    {
        StringBuilder batch = new();
        string? line;
        using var reader = new StringReader(sql);
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Trim() is "GO" or "GO;")
            {
                if (batch.Length != 0)
                    yield return batch.ToString();
                batch.Clear();
            }
            else
            {
                batch.AppendLine(line);
            }
        }
        if (batch.Length != 0)
        {
            yield return batch.ToString();
        }
    }



}
