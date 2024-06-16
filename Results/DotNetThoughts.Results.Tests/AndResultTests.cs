namespace DotNetThoughts.Results.Tests;

public class AndResultTests
{

    [Fact]
    public void HappyDays_And1()
    {
        var result = Result<int>.Ok(1)
            .And(x => Result<int>.Ok(2));

        result.Success.Should().BeTrue();
        result.Value.Should().Be((1, 2));
    }

    [Fact]
    public void HappyDays_And2()
    {
        var result = Result<int>.Ok(1)
            .And(x => Result<int>.Ok(2))
            .And((x, y) => Result<int>.Ok(3));

        result.Success.Should().BeTrue();
        result.Value.Should().Be((1, 2, 3));
    }
}