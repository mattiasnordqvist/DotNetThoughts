using System.Text;

namespace DotNetThoughts.Render.Tests;

public class TableRenderTests
{

    [Test]
    public async Task SimpleTableRenderFromObjects()
    {
        var stringBuilder = new StringBuilder();
        Render.Table.RenderTo(stringBuilder,
        [
            new { Name = "John", Age = 25 },
            new { Name = "Jane", Age = 22 },
            new { Name = "Doe", Age = 30 }
        ]);
        await Verify(stringBuilder.ToString());
    }

    [Test]
    public async Task SimpleTableRenderFromStringsList()
    {
        var stringBuilder = new StringBuilder();
        Render.Table.RenderTo(stringBuilder,
        [
            ["Name", "Age"],
            ["John", "25"],
            ["Jane", "22"],
            ["Doe", "30"]
        ]);
        await Verify(stringBuilder.ToString());
    }

    [Test]
    public async Task FitToContent()
    {
        var stringBuilder = new StringBuilder();
        Render.Table.RenderTo(stringBuilder,
        [
            new { Column = "a", Value = "1" },
        ]);
        stringBuilder.AppendLine();
        Render.Table.RenderTo(stringBuilder,
        [
            new { Column = "a", Value = "1" },
            new { Column = "aa", Value = "2" },
        ]);
        stringBuilder.AppendLine();
        Render.Table.RenderTo(stringBuilder,
        [
            new { Column = "a", Value = "1" },
            new { Column = "aa", Value = "2" },
            new { Column = "aaa", Value = "3" },
        ]);
        stringBuilder.AppendLine();
        await Verify(stringBuilder.ToString());
    }

    [Test]
    public async Task FixedWidth()
    {
        var stringBuilder = new StringBuilder();
        var table = new TableModel<List<string>>();
        table.Columns.Add(new TableModel<List<string>>.ColumnModel { Index = 0, Width = new FixedWidth(10), Header = "C1", Alignment = Alignment.Left, GetValue = (x, y, _) => y[0] });
        table.Columns.Add(new TableModel<List<string>>.ColumnModel { Index = 1, Width = new FixedWidth(5), Header = "C2", Alignment = Alignment.Left, GetValue = (x, y, _) => y[1] });
        table.Columns.Add(new TableModel<List<string>>.ColumnModel { Index = 2, Width = new FixedWidth(5), Header = "C3", Alignment = Alignment.Right, GetValue = (x, y, _) => y[2] });

        stringBuilder.AppendLine("C1 has fixed width 10");
        stringBuilder.AppendLine("C2 has fixed width 5");
        stringBuilder.AppendLine("C3 has fixed width 5");
        stringBuilder.AppendLine();

        table.Rows = [["1", "1", "1"]];
        table.RenderTo(stringBuilder);
        stringBuilder.AppendLine();

        table.Rows = [["12345", "12345", "12345"]];
        table.RenderTo(stringBuilder);
        stringBuilder.AppendLine();

        table.Rows = [["0123456789", "0123456789", "0123456789"]];
        table.RenderTo(stringBuilder);
        stringBuilder.AppendLine();

        await Verify(stringBuilder.ToString());
    }

    [Test]
    public async Task RowIndex()
    {
        var table = new TableModel<string>();
        table.Columns.Add(TableModel<string>.ColumnModel.Create(0, "Index", (cm, row, index) => index, configure: cm => cm.Alignment = Alignment.Right));
        table.Columns.Add(new TableModel<string>.ColumnModel { Index = 1, Width = new FitToContent(), Header = "Value", Alignment = Alignment.Left, GetValue = (x, y, ri) => y });

        table.Rows = ["Row 1", "Row 2", "Row 3"];
        var stringBuilder = new StringBuilder();
        table.RenderTo(stringBuilder);
        await Verify(stringBuilder.ToString());
    }
}
