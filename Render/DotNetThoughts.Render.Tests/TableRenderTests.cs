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
}
