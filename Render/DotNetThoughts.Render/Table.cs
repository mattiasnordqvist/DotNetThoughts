using System.Reflection;
using System.Text;

namespace DotNetThoughts.Render;

public class Table
{
    private List<Column> _columns = [];

    private class Column
    {
        public required string Header { get; set; }
        public required Alignment Alignment { get; set; }

        public Func<object?, string> Render { get; set; } = DefaultRender;

        public static Func<object?, string> DefaultRender = (value) => value?.ToString() ?? "";
    }
    internal enum Alignment
    {
        Right,
        Left
    }

    public static Table CreateFrom<T>()
    {
        var table = new Table();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        table._columns = properties.Select(p => new Column
        {
            Header = p.Name,
            Alignment = IsNumericType(p.PropertyType) ? Alignment.Right : Alignment.Left
        }).ToList();
        return table;
    }

    public string Render<T>(IEnumerable<T> items)
    {
        var stringBuilder = new StringBuilder();
        this.RenderTo(stringBuilder, items);
        return stringBuilder.ToString();
    }

    public void RenderTo<T>(StringBuilder stringBuilder, IEnumerable<T> items)
    {
        var headers = _columns.Select(c => c.Header).ToArray();

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var renderedRows = items.Select(item => _columns.Select(c => c.Render(properties.FirstOrDefault(p => p.Name == c.Header)?.GetValue(item, null))).ToArray()).ToList();

        var calculatedColumnWidths = headers.Select((header, index) =>
        {
            var width = Math.Max(header.Length, renderedRows.Max(row => row[index].Length));
            return width;
        }).ToArray();

        var columnRenderInfo = _columns.Select((c, index) => new ColumnRenderInfo
        {
            Alignment = c.Alignment,
            Width = calculatedColumnWidths[index]
        }).ToArray();

        AppendRow(headers, columnRenderInfo, stringBuilder);
        AppendSeparator(columnRenderInfo, stringBuilder);
        foreach (var renderedRow in renderedRows)
        {
            AppendRow(renderedRow, columnRenderInfo, stringBuilder);
        }
    }

    private class ColumnRenderInfo
    {
        public Alignment Alignment { get; set; }
        public int Width { get; set; }
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }

    private static void AppendRow(string[] row, ColumnRenderInfo[] columns, StringBuilder sb)
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

    private static void AppendSeparator(ColumnRenderInfo[] columnWidths, StringBuilder sb)
    {
        foreach (var width in columnWidths)
        {
            sb.Append("+");
            sb.Append(new string('=', width.Width + 2)); // 2 for padding
        }
        sb.AppendLine("+");
    }
    public static string RenderTable<T>(IEnumerable<T> items)
    {
        var stringBuilder = new StringBuilder();
        RenderTableTo(stringBuilder, items);
        return stringBuilder.ToString();
    }

    public static void RenderTableTo<T>(StringBuilder stringBuilder, IEnumerable<T> items)
    {
        Table.CreateFrom<T>().RenderTo(stringBuilder, items);
    }
}

