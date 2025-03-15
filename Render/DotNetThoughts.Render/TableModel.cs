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

    public List<TRow> Rows = [];

    public class ColumnModel
    {
        public required int Index { get; set; }
        public required string Header { get; set; }
        public required Alignment Alignment { get; set; }

        public required Width Width { get; set; }

        public required Func<ColumnModel, TRow, int, object?> GetValue { get; set; }
        public Func<object?, TRow, int, string> RenderValue { get; set; } = DefaultRenderValue;

        public static Func<object?, TRow, int, string> DefaultRenderValue = (value, _, _) => value?.ToString() ?? "";


        public static ColumnModel Create<TValue>(
           int columnIndex,
           string header,
           Func<ColumnModel, TRow, int, TValue?> getValue,
           Func<TValue?, TRow, int, string>? renderValue = null,
           Action<ColumnModel>? configure = null)
        {
            var columnModel = new ColumnModel
            {
                Index = columnIndex,
                Header = header,
                Width = new FitToContent(),
                Alignment = Alignment.Left,
                GetValue = (info, row, rowIndex) => getValue(info, row, rowIndex),
            };
            if (renderValue != null)
            {
                columnModel.RenderValue = (value, row, rowIndex) => renderValue((TValue?)value, row, rowIndex);
            }
            configure?.Invoke(columnModel);
            return columnModel;
        }
    }


    public static TableModel<List<string>> CreateFrom(List<List<string>> headerRowAndDataRows)
    {
        var table = new TableModel<List<string>>();

        var headers = headerRowAndDataRows.First();
        var rows = headerRowAndDataRows.Skip(1).ToList();
        table.Columns = headers.Select((h,i) => new TableModel<List<string>>.ColumnModel
        {
            Index = i,
            Header = h,
            Width = new FitToContent(),
            Alignment = rows.Select(x => x[i]).All(IsNumeric) ? Alignment.Right : Alignment.Left,
            GetValue = (info, row, _) => row[info.Index],

        }).ToList();
        table.Rows = rows;
        return table;

        bool IsNumeric(string value) => double.TryParse(value, out _);
    }

    public static TableModel<T> CreateFrom<T>(List<T> rows)
    {
        var table = new TableModel<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        table.Columns = properties.Select((p,i) => new TableModel<T>.ColumnModel
        {
            Index = i,
            Header = p.Name,
            Width = new FitToContent(),
            Alignment = IsNumericType(p.PropertyType) ? Alignment.Right : Alignment.Left,
            GetValue = (info, row, _) => p.GetValue(row, null),

        }).ToList();
        table.Rows = rows;
        return table;
    }

    public string Render()
    {
        var stringBuilder = new StringBuilder();
        this.RenderTo(stringBuilder);
        return stringBuilder.ToString();
    }


    public void RenderTo(StringBuilder stringBuilder) 
    {
        var headers = Columns.Select(x => x.Header).ToArray();
        var renderedRows = Rows.Select((row, rowIndex) => Columns.Select(c => c.RenderValue(c.GetValue(c, row, rowIndex), row, rowIndex)).ToArray());

        var calculatedColumnWidths = Columns.Select((column, index) =>
        {
            if(column.Width is FixedWidth fixedWidth)
            {
                return fixedWidth.Width;
            }
            else if (column.Width is FitToContent)
            {
                return Math.Max(column.Header.Length, renderedRows.Max(row => row[index].Length));
            }
            else
            {
                throw new NotSupportedException($"Width type {column.Width.GetType().Name} is not supported");
            }
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
                var content = row[i];
                var padded = content.PadLeft(columns[i].Width);
                var constrained = padded[(padded.Length - columns[i].Width)..];
                sb.Append($"| {constrained} ");
            }
            else
            {
                sb.Append($"| {row[i].PadRight(columns[i].Width)[0..columns[i].Width]} ");
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
            var table = TableModel<T>.CreateFrom(items);
            table.RenderTo(stringBuilder);
        }


        public static string Render(List<string> items)
        {
            var stringBuilder = new StringBuilder();
            RenderTo(stringBuilder, items);
            return stringBuilder.ToString();

        }
        public static void RenderTo(StringBuilder stringBuilder, List<List<string>> items)
        {
            var table = TableModel<List<string>>.CreateFrom(items);
            table.RenderTo(stringBuilder);
        }
    }
}
