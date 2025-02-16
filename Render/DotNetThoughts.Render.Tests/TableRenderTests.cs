using System.Text;

namespace DotNetThoughts.Render.Tests;

public class TableRenderTests
{

    [Test]
    public async Task SimpleTableRender()
    {
        var stringBuilder = new StringBuilder();
        Table.RenderTo(stringBuilder,
        [
            new { Name = "John", Age = 25 },
            new { Name = "Jane", Age = 22 },
            new { Name = "Doe", Age = 30 }
        ]);
        await Verify(stringBuilder.ToString());
    }
}
