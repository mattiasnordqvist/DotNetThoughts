namespace DotNetThoughts.Results.Tests;

public class BindTests
{
    [Test]
    [Arguments(123)]
    [Arguments(null)]
    public async Task ReturnWrapsInSuccessResult(object? value)
    {
        value.Return().Success.ShouldBeTrue();
        value.Return().Value.ShouldBe(value);
    }

    [Test]
    public async Task BindTransfersValueToLastInChain()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Ok(2))
            .Value.ShouldBe(2);
    }

    [Test]
    public async Task BindTransfersValueToLastInChainWhenBindingDeep()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1)
                .Bind(x => Result<int>.Ok(2)))
            .Value.ShouldBe(2);
    }

    [Test]
    public async Task BindReturnsErrorIfBeginsWithError()
    {
        Result<object>.Error(new FakeError())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Ok(2))
            .Success.ShouldBeFalse();
    }
    [Test]
    public async Task BindReturnsErrorIfEndsWithError()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Ok(1))
            .Bind(x => Result<int>.Error(new FakeError()))
            .Success.ShouldBeFalse();
    }

    [Test]
    public async Task BindReturnsErrorIfErrorInMiddle()
    {
        Result<object>.Ok(new object())
            .Bind(x => Result<int>.Error(new FakeError()))
            .Bind(x => Result<int>.Ok(2))
            .Success.ShouldBeFalse();
    }

    [Test]
    public async Task BindPassesValueCorrectly()
    {
        Result<int>.Ok(1)
            .Bind(x => Result<int>.Ok(x + 1))
            .Bind(x => Result<int>.Ok(x + 1))
            .Value.ShouldBe(3);
    }

    [Test]
    public async Task BindPassesValueCorrectlyWhenBindingInside()
    {
        var result = Result<int>.Ok(1)
            .Bind(x => Result<int>.Ok(x + 1)
                .Bind(x => Result<int>.Ok(x + 1)));
        await Assert.That(result.Value).IsEqualTo(3);
    }

    [Test]
    public async Task BindWith2Tuple()
    {
        var result = Result<(int, int)>.Ok((0, 10))
            .Bind((x, y) => Result<(int, int)>.Ok((x + 1, y + 1)))
            .Bind((x, y) => Result<(int, int)>.Ok((x + 1, y + 1)));
        await Assert.That(result.Value).IsEqualTo((2, 12));
    }

    [Test]
    public async Task BindWith3Tuple()
    {
        Result<(int, int, decimal)>.Ok((0, 10, 100m))
            .Bind((x, y, z) => Result<(int, int, decimal)>.Ok((x + 1, y + 1, z + 1)))
            .Bind((x, y, z) => Result<(int, int, decimal)>.Ok((x + 1, y + 1, z + 1)))
            .Value.ShouldBe((2, 12, 102m));
    }

    [Test]
    public async Task BindWith4Tuple()
    {
        Result<(int, int, decimal, bool)>.Ok((0, 10, 100m, true))
            .Bind((x, y, z, w) => Result<(int, int, decimal, bool)>.Ok((x + 1, y + 1, z + 1, true)))
            .Bind((x, y, z, w) => Result<(int, int, decimal, bool)>.Ok((x + 1, y + 1, z + 1, true)))
            .Value.ShouldBe((2, 12, 102m, true));
    }
}