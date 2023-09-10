using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class ReturnTests
{
    [Theory]
    [InlineData(123)]
    [InlineData(null)]
    public void ReturnWrapsInSuccessResult(object? value)
    {
        value.Return().Success.Should().BeTrue();
        value.Return().Value.Should().Be(value);
    }

    [Theory]
    [InlineData(123)]
    [InlineData(null)]
    public async Task ReturnWrapsInSuccessResult_TaskVersion(object? value)
    {
        (await Task.FromResult(value).Return()).Success.Should().BeTrue();
        (await Task.FromResult(value).Return()).Value.Should().Be(value);
    }
}