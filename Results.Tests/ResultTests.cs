using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class ResultTests
{
    [Theory]
    [InlineData((object?)null)]
    [InlineData(new[] { 1 })]
    [InlineData(123)]
    public void OkResultsAreSuccess(object? value)
    {
        Result<object?>.Ok(value).Success.Should().BeTrue();
        Result<object?>.Ok(value).Value.Should().Be(value);
    }

    [Fact]
    public void ErrorResultsAreNotSuccess()
    {
        Result<object>.Error(new FakeError()).Success.Should().BeFalse();
    }

    [Fact]
    public void AccessingValueOnErrorResultShouldThrowException()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValueOrThrowOnErrorResultShouldThrowExceptionWithErrorsInside()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).ValueOrThrow();
        act.Should().Throw<ValueOrThrowException>().Which.Errors.Should().ContainEquivalentOf(new FakeError());
    }

    [Fact]
    public void CreateErrorResultWithoutErrorsShouldThrowException()
    {
        Func<Result<object>> act = () => Result<object>.Error(Array.Empty<ErrorBase>());
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ErrorResultWithMultipleErrorsShouldRetainAllErrors()
    {
        Result<object>.Error(new FakeError(), new FakeError(), new FakeError())
            .Errors.Count().Should().Be(3);
    }

    [Fact]
    public void ErrorResultWithMultipleErrorsAsListShouldRetainAllErrors()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError(), new FakeError() })
            .Errors.Count.Should().Be(3);
    }

    [Fact]
    public void IsErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult()
    {
        var firstError = new FakeError();
        Result<object>.Error(new List<FakeError>() { firstError, new FakeError(), new FakeError(), new FakeError() })
            .IsError<FakeError>().Should().BeSameAs(firstError);
    }

    [Fact]
    public void IsErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult_OutParameterVersion()
    {
        var firstError = new FakeError();
        var result = Result<object>.Error(new List<IError>() { firstError, new FakeError(), new AnotherError(), new AnotherError() });
        var isError = result.IsError<FakeError>(out var error);
        isError.Should().BeTrue();
        error.Should().BeSameAs(firstError);
    }

    [Fact]
    public void IsErrorCorrectlyAnswersWithNullWhenErrorIsNotPresentInResult()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError() })
            .IsError<AnotherError>().Should().Be(null);
    }

    [Fact]
    public void IsErrorCorrectlyAnswersWithNullWhenResultIsSuccess()
    {
        Result<object>.Ok(null!)
            .IsError<FakeError>().Should().Be(null);
    }

    [Fact]
    public void CastingResultToTaskOfResultRetainsResultValue()
    {
        Task<Result<object>> casted = Result<object>.Ok(1345);
        casted.Result.Value.Should().Be(1345);
        casted.Result.Success.Should().BeTrue();
    }

    [Fact]
    public void CastingOkResultToUnitResultReplacesValueButKeepsSuccess()
    {
        Result<Unit> casted = Result<object>.Ok(1345);
        casted.Value.Should().Be(Unit.Instance);
        casted.Success.Should().BeTrue();
    }

    [Fact]
    public void CastingErrorResultToUnitResultReplacesValueButKeepsErrors()
    {
        Result<Unit> casted = Result<object>.Error(new FakeError());

        casted.Success.Should().BeFalse();
        casted.IsError<FakeError>().Should().NotBeNull();
    }

    [Fact]
    public void ModifyingAPassedListOfErrorsDoesNotModifyResult()
    {
        var listOfErrors = new List<IError>() { new FakeError(), new FakeError(), new FakeError() };
        var result = Result<object>.Error(listOfErrors);
        listOfErrors.Add(new FakeError());
        result.Errors.Count.Should().Be(3);
    }
}

record FakeError : ErrorBase
{

}

record AnotherError : ErrorBase
{

}