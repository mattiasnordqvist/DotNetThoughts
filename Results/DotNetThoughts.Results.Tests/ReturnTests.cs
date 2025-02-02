namespace DotNetThoughts.Results.Tests;

public class ReturnTests
{
    [Theory]
    [InlineData(123)]
    [InlineData(null)]
    public void ReturnWrapsInSuccessResult(object? value)
    {
        value.Return().Success.ShouldBeTrue();
        value.Return().Value.ShouldBe(value);
    }

    [Theory]
    [InlineData(123)]
    [InlineData(null)]
    public async Task ReturnWrapsInSuccessResult_TaskVersion(object? value)
    {
        (await Task.FromResult(value).Return()).Success.ShouldBeTrue();
        (await Task.FromResult(value).Return()).Value.ShouldBe(value);
    }
}