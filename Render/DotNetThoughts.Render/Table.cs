using System.Reflection;
using System.Text;

namespace DotNetThoughts.Render;

public class Table
{
    public static string Render<T>(IEnumerable<T> items)
    {
        var stringBuilder = new StringBuilder();
        RenderTo(stringBuilder, items);
        return stringBuilder.ToString();
    }

    public static void RenderTo<T>(StringBuilder stringBuilder, IEnumerable<T> items)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var headers = properties.Select(p => p.Name).ToArray();
        var rowsData = items.Select(item => properties.Select(p => p.GetValue(item, null)).ToArray()).ToList();
        var renderedRows = rowsData.Select(row => row.Select(cell => cell?.ToString() ?? "").ToArray()).ToList();

        var columns = headers
            .Select((header, index) =>
            {
                var widht = Math.Max(header.Length, renderedRows.Max(row => row[index].Length));
                var alignment = IsNumericType(properties[index].PropertyType) ? Alignment.Right : Alignment.Left;
                return new Column
                {
                    Header = header,
                    Width = widht,
                    Alignment = alignment,
                };
            })
            .ToArray();

        AppendRow(headers, columns, stringBuilder);
        AppendSeparator(columns.Select(c => c.Width).ToArray(), stringBuilder);

        foreach (var row in renderedRows)
        {
            AppendRow(row, columns, stringBuilder);
        }
    }

    private static void AppendRow(string[] row, Column[] columns, StringBuilder sb)
    {
        for (int i = 0; i < row.Length; i++)
        {
            // Check if the property is numeric to align it to the right
            if (columns[i].Alignment == Alignment.Right)
            {
                sb.Append($"| {row[i].PadLeft(columns[i].Width)} ");
            }
            else
            {
                sb.Append($"| {row[i].PadRight(columns[i].Width)} ");
            }
        }
        sb.AppendLine("|");
    }

    private static void AppendSeparator(int[] columnWidths, StringBuilder sb)
    {
        foreach (var width in columnWidths)
        {
            sb.Append("+");
            sb.Append(new string('=', width + 2)); // 2 for padding
        }
        sb.AppendLine("+");
    }

    private class Column
    {
        public required string Header { get; set; }
        public required int Width { get; set; }
        public required Alignment Alignment { get; set; }
    }

    // Helper function to check if a type is numeric
    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }
    internal enum Alignment
    {
        Right,
        Left
    }
}

