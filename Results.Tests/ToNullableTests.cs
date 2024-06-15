using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class ToNullableTests
{
    [Fact]
    public void ToNullableCompilesAndDoesNotChangeValue()
    {
        var a = Result<int>.Ok(123);
        Result<int?> wishThisCouldBeCastedInstead = a.ToNullableStruct();
        wishThisCouldBeCastedInstead.Value.Should().Be(123);

        var a2 = Result<string>.Ok("123");
        Result<string?> wishThisCouldBeCastedInstead2 = a2.ToNullable();
        wishThisCouldBeCastedInstead2.Value.Should().Be("123");
    }

}
