namespace DotNetThoughts.Results.Tests;

public class ReturnTests
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
    [Arguments(123)]
    [Arguments(null)]
    public async Task ReturnWrapsInSuccessResult_TaskVersion(object? value)
    {
        (await Task.FromResult(value).Return()).Success.ShouldBeTrue();
        (await Task.FromResult(value).Return()).Value.ShouldBe(value);
    }
}