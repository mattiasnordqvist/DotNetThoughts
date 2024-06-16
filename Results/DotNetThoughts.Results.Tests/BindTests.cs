using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class BindTests
{
    [Theory]
    [InlineData(123)]
    [InlineData(null)]
    public void ReturnWrapsInSuccessResult(object? value)
    {
        value.Return().Success.Should().BeTrue();
        value.Return().Value.Should().Be(value);
    }

    [Fact]
    public void BindTransfersValueToLastInChain()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Ok(2))
            .Value.Should().Be(2);
    }

    [Fact]
    public void BindTransfersValueToLastInChainWhenBindingDeep()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1)
                .Bind(x => Result<int>.Ok(2)))
            .Value.Should().Be(2);
    }

    [Fact]
    public void BindReturnsErrorIfBeginsWithError()
    {
        Result<object>.Error(new FakeError())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Ok(2))
            .Success.Should().BeFalse();
    }
    [Fact]
    public void BindReturnsErrorIfEndsWithError()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Error(new FakeError()))
            .Success.Should().BeFalse();
    }

    [Fact]
    public void BindReturnsErrorIfErrorInMiddle()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Error(new FakeError()))
            .Bind(x => Result<int>.Ok(2))
            .Success.Should().BeFalse();
    }

    [Fact]
    public void BindPassesValueCorrectly()
    {
        Result<int>.Ok(1)
            .Bind(x => Result<int>.Ok(x + 1))
            .Bind(x => Result<int>.Ok(x + 1))
            .Value.Should().Be(3);
    }

    [Fact]
    public void BindPassesValueCorrectlyWhenBindingInside()
    {
        Result<int>.Ok(1)
            .Bind(x => Result<int>.Ok(x + 1)
                .Bind(x => Result<int>.Ok(x + 1)))
            .Value.Should().Be(3);
    }

    [Fact]
    public void BindWith2Tuple()
    {
        Result<(int, int)>.Ok((0, 10))
            .Bind((x, y) => Result<(int, int)>.Ok((x + 1, y + 1)))
            .Bind((x, y) => Result<(int, int)>.Ok((x + 1, y + 1)))
            .Value.Should().Be((2, 12));
    }

    [Fact]
    public void BindWith3Tuple()
    {
        Result<(int, int, decimal)>.Ok((0, 10, 100m))
            .Bind((x, y, z) => Result<(int, int, decimal)>.Ok((x + 1, y + 1, z + 1)))
            .Bind((x, y, z) => Result<(int, int, decimal)>.Ok((x + 1, y + 1, z + 1)))
            .Value.Should().Be((2, 12, 102m));
    }

    [Fact]
    public void BindWith4Tuple()
    {
        Result<(int, int, decimal, bool)>.Ok((0, 10, 100m, true))
            .Bind((x, y, z, w) => Result<(int, int, decimal, bool)>.Ok((x + 1, y + 1, z + 1, true)))
            .Bind((x, y, z, w) => Result<(int, int, decimal, bool)>.Ok((x + 1, y + 1, z + 1, true)))
            .Value.Should().Be((2, 12, 102m, true));
    }
}