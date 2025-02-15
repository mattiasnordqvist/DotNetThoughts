namespace DotNetThoughts.Results.Tests;

public class AndResultTests
{

    [Test]
    public async Task HappyDays_And1()
    {
        var result = Result<int>.Ok(1)
            .And(x => Result<int>.Ok(2));
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((1, 2));
    }

    [Test]
    public async Task HappyDays_And2()
    {
        var result = Result<int>.Ok(1)
            .And(x => Result<int>.Ok(2))
            .And((x, y) => Result<int>.Ok(3));

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((1, 2, 3));
    }
}