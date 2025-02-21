using System.Reflection;
using System.Text;

namespace DotNetThoughts.Render;

public enum Alignment
{
    Right,
    Left
}
public interface Width
{
}

public class FitToContent : Width
{
}

public class FixedWidth : Width
{
    public FixedWidth(int width)
    {
        Width = width;
    }

    public int Width { get; set; }
}

public class TableModel<TRow>
{
    public List<ColumnModel> Columns = [];

    public class ColumnModel
    {
        public required int Index { get; set; }
        public required string Header { get; set; }
        public required Alignment Alignment { get; set; }

        public required Width Width { get; set; }

        public required Func<ColumnModel, TRow, object?> GetValue { get; set; }
        public Func<object?, TRow, string> RenderValue { get; set; } = DefaultRenderValue;

        public static Func<object?, TRow, string> DefaultRenderValue = (value, _) => value?.ToString() ?? "";
    }


    public static TableModel<List<string>> CreateFrom(List<string> headers)
    {
        var table = new TableModel<List<string>>();
        table.Columns = headers.Select(static (h,i) => new TableModel<List<string>>.ColumnModel
        {
            Index = i,
            Header = h,
            Width = new FitToContent(),
            Alignment = Alignment.Left,
            GetValue = (info, row) => row[info.Index],

        }).ToList();
        return table;
    }

    public static TableModel<T> CreateFrom<T>()
    {
        var table = new TableModel<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        table.Columns = properties.Select((p,i) => new TableModel<T>.ColumnModel
        {
            Index = i,
            Header = p.Name,
            Width = new FitToContent(),
            Alignment = IsNumericType(p.PropertyType) ? Alignment.Right : Alignment.Left,
            GetValue = (info, row) => p.GetValue(row, null),

        }).ToList();
        return table;
    }

    public string Render(List<TRow> items)
    {
        var stringBuilder = new StringBuilder();
        this.RenderTo(stringBuilder, items);
        return stringBuilder.ToString();
    }


    public void RenderTo(StringBuilder stringBuilder, List<TRow> items) 
    {
        var headers = Columns.Select(x => x.Header).ToArray();
        var renderedRows = items.Select(item => Columns.Select(c => c.RenderValue(c.GetValue(c, item), item)).ToArray());

        var calculatedColumnWidths = headers.Select((header, index) =>
        {
            var width = Math.Max(header.Length, renderedRows.Max(row => row[index].Length));
            return width;
        }).ToArray();

        var columnRenderInfo = Columns.Select((c, index) => new ColumnRenderInfo
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


    public class ColumnRenderInfo
    {
        public required Alignment Alignment { get; set; }
        public required int Width { get; set; }
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

}

public static class Render
{
    public static class Table
    {
        public static string Render<T>(List<T> items)
        {
            var stringBuilder = new StringBuilder();
            RenderTo(stringBuilder, items);
            return stringBuilder.ToString();

        }
        public static void RenderTo<T>(StringBuilder stringBuilder, List<T> items)
        {
            var table = TableModel<T>.CreateFrom<T>();
            table.RenderTo(stringBuilder, items);
        }


        public static string Render(List<string> items)
        {
            var stringBuilder = new StringBuilder();
            RenderTo(stringBuilder, items);
            return stringBuilder.ToString();

        }
        public static void RenderTo(StringBuilder stringBuilder, List<List<string>> items)
        {
            var table = TableModel<List<string>>.CreateFrom(items.First());
            table.RenderTo(stringBuilder, items.Skip(1).ToList());
        }
    }
}
