namespace DotNetThoughts.Results.Tests;

public class MapTests
{
    [Test]
    public async Task MapTransfersValueToLastInChain()
    {
        Result<object>.Ok(new object())
            .Map(x => 1)
            .Map(x => 2)
            .Value.ShouldBe(2);
    }

    [Test]
    public async Task MapFromTaskTransfersValueToLastInChain()
    {
        (await Task.FromResult(Result<object>.Ok(new object()))
            .Map(x => 1)
            .Map(x => 2))
            .Value.ShouldBe(2);
    }

    [Test]
    public async Task MapReturnsErrorIfBeginsWithError()
    {
        Result<object>.Error(new FakeError())
            .Map(x => 1)
            .Map(x => 2)
            .Success.ShouldBeFalse();
    }
    [Test]
    public async Task MapReturnsErrorIfEndsWithError()
    {
        Result<object>.Ok(new object())
            .Map(x => 1)
            .Bind(x => Result<int>.Error(new FakeError()))
            .Success.ShouldBeFalse();
    }

    [Test]
    public async Task MapReturnsErrorIfErrorInMiddle()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Error(new FakeError()))
            .Map(x => 2)
            .Success.ShouldBeFalse();
    }

    [Test]
    public async Task MapPassesValueCorrectly()
    {
        Result<int>.Ok(1)
            .Map(x => x + 1)
            .Map(x => x + 1)
            .Value.ShouldBe(3);
    }

    [Test]
    public async Task MapWith2Tuple()
    {
        Result<(int, int)>.Ok((0, 10))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1))
            .Value.ShouldBe((2, 12));
    }

    [Test]
    public async Task MapWith2Tuple_FromTask()
    {
        (await Task.FromResult(Result<(int, int)>.Ok((0, 10)))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1)))
            .Value.ShouldBe((2, 12));
    }

    [Test]
    public async Task MapWith3Tuple()
    {
        Result<(int, int, decimal)>.Ok((0, 10, 100m))
            .Map((x, y, z) => (x + 1, y + 1, z + 1))
            .Map((x, y, z) => (x + 1, y + 1, z + 1))
            .Value.ShouldBe((2, 12, 102m));
    }
}