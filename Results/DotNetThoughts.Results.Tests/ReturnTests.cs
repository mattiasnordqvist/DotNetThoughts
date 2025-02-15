namespace DotNetThoughts.Results.Tests;

public class ReturnTests
{
    [Test]
    [Arguments(123)]
    [Arguments(null)]
    public async Task ReturnWrapsInSuccessResult(object? value)
    {
        var result = value.Return();
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(value);
    }

    [Test]
    [Arguments(123)]
    [Arguments(null)]
    public async Task ReturnWrapsInSuccessResult_TaskVersion(object? value)
    {
        var result = await Task.FromResult(value).Return();
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(value);
    }
}