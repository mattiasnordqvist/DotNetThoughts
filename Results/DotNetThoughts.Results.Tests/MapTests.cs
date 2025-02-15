namespace DotNetThoughts.Results.Tests;

public class MapTests
{
    [Test]
    public async Task MapTransfersValueToLastInChain()
    {
        var result = Result<object>.Ok(new object())
            .Map(x => 1)
            .Map(x => 2);
        await Assert.That(result.Value).IsEqualTo(2);
    }

    [Test]
    public async Task MapFromTaskTransfersValueToLastInChain()
    {
        var result = await Task.FromResult(Result<object>.Ok(new object()))
            .Map(x => 1)
            .Map(x => 2);
        await Assert.That(result.Value).IsEqualTo(2);
    }

    [Test]
    public async Task MapReturnsErrorIfBeginsWithError()
    {
        var result = Result<object>.Error(new FakeError())
            .Map(x => 1)
            .Map(x => 2);
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task MapReturnsErrorIfEndsWithError()
    {
        var result = Result<object>.Ok(new object())
            .Map(x => 1)
            .Bind(x => Result<int>.Error(new FakeError()));
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task MapReturnsErrorIfErrorInMiddle()
    {
        var result = Result<object>.Ok(new object())
            .Bind(x => Result<int>.Error(new FakeError()))
            .Map(x => 2);
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task MapPassesValueCorrectly()
    {
        var result = Result<int>.Ok(1)
            .Map(x => x + 1)
            .Map(x => x + 1);
        await Assert.That(result.Value).IsEqualTo(3);
    }

    [Test]
    public async Task MapWith2Tuple()
    {
        var result = Result<(int, int)>.Ok((0, 10))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1));
        await Assert.That(result.Value).IsEqualTo((2, 12));
    }

    [Test]
    public async Task MapWith2Tuple_FromTask()
    {
        var result = await Task.FromResult(Result<(int, int)>.Ok((0, 10)))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1));
        await Assert.That(result.Value).IsEqualTo((2, 12));
    }

    [Test]
    public async Task MapWith3Tuple()
    {
        var result = Result<(int, int, decimal)>.Ok((0, 10, 100m))
            .Map((x, y, z) => (x + 1, y + 1, z + 1))
            .Map((x, y, z) => (x + 1, y + 1, z + 1));
        await Assert.That(result.Value).IsEqualTo((2, 12, 102m));
    }
}