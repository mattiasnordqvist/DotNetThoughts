using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class MapTests
{
    [Fact]
    public void MapTransfersValueToLastInChain()
    {
        Result<object>.Ok(new object())
            .Map(x => 1)
            .Map(x => 2)
            .Value.Should().Be(2);
    }

    [Fact]
    public async Task MapFromTaskTransfersValueToLastInChain()
    {
        (await Task.FromResult(Result<object>.Ok(new object()))
            .Map(x => 1)
            .Map(x => 2))
            .Value.Should().Be(2);
    }

    [Fact]
    public void MapReturnsErrorIfBeginsWithError()
    {
        Result<object>.Error(new FakeError())
            .Map(x => 1)
            .Map(x => 2)
            .Success.Should().BeFalse();
    }
    [Fact]
    public void MapReturnsErrorIfEndsWithError()
    {
        Result<object>.Ok(new object())
            .Map(x => 1)
            .Bind(x => Result<int>.Error(new FakeError()))
            .Success.Should().BeFalse();
    }

    [Fact]
    public void MapReturnsErrorIfErrorInMiddle()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Error(new FakeError()))
            .Map(x => 2)
            .Success.Should().BeFalse();
    }

    [Fact]
    public void MapPassesValueCorrectly()
    {
        Result<int>.Ok(1)
            .Map(x => x + 1)
            .Map(x => x + 1)
            .Value.Should().Be(3);
    }

    [Fact]
    public void MapWith2Tuple()
    {
        Result<(int, int)>.Ok((0, 10))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1))
            .Value.Should().Be((2, 12));
    }

    [Fact]
    public async Task MapWith2Tuple_FromTask()
    {
        (await Task.FromResult(Result<(int, int)>.Ok((0, 10)))
            .Map((x, y) => (x + 1, y + 1))
            .Map((x, y) => (x + 1, y + 1)))
            .Value.Should().Be((2, 12));
    }

    [Fact]
    public void MapWith3Tuple()
    {
        Result<(int, int, decimal)>.Ok((0, 10, 100m))
            .Map((x, y, z) => (x + 1, y + 1, z + 1))
            .Map((x, y, z) => (x + 1, y + 1, z + 1))
            .Value.Should().Be((2, 12, 102m));
    }
}