using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class UnitTests
{
    [Fact]
    public void UnitIsSingleInstance()
    {
        Unit.Instance.Should().BeSameAs(Unit.Instance);
    }

    [Fact]
    public void UnitIsSingleInstance_2()
    {
        Result<Unit> result = Result<int>.Ok(23);
        Result<Unit> result2 = Result<decimal>.Ok(65.6m);
        result.Value.Should().BeSameAs(result2.Value);
    }
}
