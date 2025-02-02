namespace DotNetThoughts.Results.Tests;

public class ResultTests
{
    [Theory]
    [InlineData((object?)null)]
    [InlineData(new[] { 1 })]
    [InlineData(123)]
    public void OkResultsAreSuccess(object? value)
    {
        Result<object?>.Ok(value).Success.ShouldBeTrue();
        Result<object?>.Ok(value).Value.ShouldBe(value);
    }

    [Fact]
    public void ErrorResultsAreNotSuccess()
    {
        Result<object>.Error(new FakeError()).Success.ShouldBeFalse();
    }

    [Fact]
    public void AccessingValueOnErrorResultShouldThrowException()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).Value;
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void ValueOrThrowOnErrorResultShouldThrowExceptionWithErrorsInside()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).ValueOrThrow();
        act.ShouldThrow<ValueOrThrowException>("hello");
    }

    [Fact]
    public void CreateErrorResultWithoutErrorsShouldThrowException()
    {
        Action act = () => Result<object>.Error([]);
        act.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void ErrorResultWithMultipleErrorsShouldRetainAllErrors()
    {
        Result<object>.Error(new FakeError(), new FakeError(), new FakeError())
            .Errors.Count().ShouldBe(3);
    }

    [Fact]
    public void ErrorResultWithMultipleErrorsAsListShouldRetainAllErrors()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError(), new FakeError() })
            .Errors.Count.ShouldBe(3);
    }

    [Fact]
    public void HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult()
    {
        var firstError = new FakeError();
        Result<object>.Error(new List<FakeError>() { firstError, new FakeError(), new FakeError(), new FakeError() })
            .HasError<FakeError>(out var error);
        error.ShouldBeSameAs(firstError);
    }

    [Fact]
    public void HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult_OutParameterVersion()
    {
        var firstError = new FakeError();
        var result = Result<object>.Error([firstError, new FakeError(), new AnotherError(), new AnotherError()]);
        var isError = result.HasError<FakeError>(out var error);
        isError.ShouldBeTrue();
        error.ShouldBeSameAs(firstError);
    }

    [Fact]
    public void HasErrorCorrectlyAnswersWithNullWhenErrorIsNotPresentInResult()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError() })
            .HasError<AnotherError>().ShouldBe(false);
    }

    [Fact]
    public void HasErrorCorrectlyAnswersWithNullWhenResultIsSuccess()
    {
        Result<object>.Ok(null!)
            .HasError<FakeError>(out var noError).ShouldBe(false);
        noError.ShouldBe(null);
    }

    [Fact]
    public void CastingResultToTaskOfResultRetainsResultValue()
    {
        Task<Result<object>> casted = Result<object>.Ok(1345);
        casted.Result.Value.ShouldBe(1345);
        casted.Result.Success.ShouldBeTrue();
    }

    [Fact]
    public void CastingOkResultToUnitResultReplacesValueButKeepsSuccess()
    {
        Result<Unit> casted = Result<object>.Ok(1345);
        casted.Value.ShouldBe(Unit.Instance);
        casted.Success.ShouldBeTrue();
    }

    [Fact]
    public void CastingErrorResultToUnitResultReplacesValueButKeepsErrors()
    {
        Result<Unit> casted = Result<object>.Error(new FakeError());

        casted.Success.ShouldBeFalse();
        casted.IsError<FakeError>().ShouldNotBeNull();
    }

    [Fact]
    public void ModifyingAPassedListOfErrorsDoesNotModifyResult()
    {
        var listOfErrors = new List<IError>() { new FakeError(), new FakeError(), new FakeError() };
        var result = Result<object>.Error(listOfErrors);
        listOfErrors.Add(new FakeError());
        result.Errors.Count.ShouldBe(3);
    }

    [Fact]
    public void ToStringReturnsExpectedResultForSuccess()
    {
        var result = Result<int>.Ok(1234);
        result.ToString().ShouldBe("Result { Success = True, Value = 1234 }");
    }

    [Fact]
    public void ToStringReturnsExpectedResultForErrorWithoutData()
    {
        var result = Result<object>.Error(new FakeError());
        result.ToString().ShouldBe("Result { Success = False, Errors = [ FakeError { Type = FakeError, Message = FakeError, Data = { } } ] }");
    }

    [Fact]
    public void ToStringReturnsExpectedResultForErrorWithData()
    {
        var result = Result<object>.Error(new ErrorWithData(5));
        result.ToString().ShouldBe("Result { Success = False, Errors = [ ErrorWithData { Type = ErrorWithData, Message = Error message number 5, Data = { Number = 5 }, Number = 5 } ] }");
    }
}

record FakeError : ErrorBase
{

}

record AnotherError : ErrorBase
{

}

record ErrorWithData(int Number) : ErrorBase($"Error message number {Number}")
{

}