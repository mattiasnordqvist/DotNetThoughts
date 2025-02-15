namespace DotNetThoughts.Results.Tests;

public class ResultTests
{
    [Test]
    [Arguments((object?)null)]
    [Arguments(new[] { 1 })]
    [Arguments(123)]
    public async Task OkResultsAreSuccess(object? value)
    {
        await Assert.That(Result<object?>.Ok(value).Success).IsTrue();
        await Assert.That(Result<object?>.Ok(value).Value).IsEqualTo(value);
    }

    [Test]
    public async Task ErrorResultsAreNotSuccess()
    {
        await Assert.That(Result<object>.Error(new FakeError()).Success).IsFalse();
    }

    [Test]
    public async Task AccessingValueOnErrorResultShouldThrowException()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).Value;
        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public async Task ValueOrThrowOnErrorResultShouldThrowExceptionWithErrorsInside()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).ValueOrThrow();
        act.ShouldThrow<ValueOrThrowException>("hello");
    }

    [Test]
    public async Task CreateErrorResultWithoutErrorsShouldThrowException()
    {
        Action act = () => Result<object>.Error([]);
        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public async Task ErrorResultWithMultipleErrorsShouldRetainAllErrors()
    {
        Result<object>.Error(new FakeError(), new FakeError(), new FakeError())
            .Errors.Count().ShouldBe(3);
    }

    [Test]
    public async Task ErrorResultWithMultipleErrorsAsListShouldRetainAllErrors()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError(), new FakeError() })
            .Errors.Count.ShouldBe(3);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult()
    {
        var firstError = new FakeError();
        Result<object>.Error(new List<FakeError>() { firstError, new FakeError(), new FakeError(), new FakeError() })
            .HasError<FakeError>(out var error);
        error.ShouldBeSameAs(firstError);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult_OutParameterVersion()
    {
        var firstError = new FakeError();
        var result = Result<object>.Error([firstError, new FakeError(), new AnotherError(), new AnotherError()]);
        var isError = result.HasError<FakeError>(out var error);
        isError.ShouldBeTrue();
        error.ShouldBeSameAs(firstError);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithNullWhenErrorIsNotPresentInResult()
    {
        Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError() })
            .HasError<AnotherError>().ShouldBe(false);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithNullWhenResultIsSuccess()
    {
        Result<object>.Ok(null!)
            .HasError<FakeError>(out var noError).ShouldBe(false);
        noError.ShouldBe(null);
    }

    [Test]
    public async Task CastingResultToTaskOfResultRetainsResultValue()
    {
        Task<Result<object>> casted = Result<object>.Ok(1345);
        casted.Result.Value.ShouldBe(1345);
        casted.await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task CastingOkResultToUnitResultReplacesValueButKeepsSuccess()
    {
        Result<Unit> casted = Result<object>.Ok(1345);
        casted.Value.ShouldBe(Unit.Instance);
        casted.Success.ShouldBeTrue();
    }

    [Test]
    public async Task CastingErrorResultToUnitResultReplacesValueButKeepsErrors()
    {
        Result<Unit> casted = Result<object>.Error(new FakeError());

        casted.Success.ShouldBeFalse();
        casted.IsError<FakeError>().ShouldNotBeNull();
    }

    [Test]
    public async Task ModifyingAPassedListOfErrorsDoesNotModifyResult()
    {
        var listOfErrors = new List<IError>() { new FakeError(), new FakeError(), new FakeError() };
        var result = Result<object>.Error(listOfErrors);
        listOfErrors.Add(new FakeError());
        result.Errors.Count.ShouldBe(3);
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForSuccess()
    {
        var result = Result<int>.Ok(1234);
        result.ToString().ShouldBe("Result { Success = True, Value = 1234 }");
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForErrorWithoutData()
    {
        var result = Result<object>.Error(new FakeError());
        result.ToString().ShouldBe("Result { Success = False, Errors = [ FakeError { Type = FakeError, Message = FakeError, Data = { } } ] }");
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForErrorWithData()
    {
        var result = Result<object>.Error(new ErrorWithData(5));
        await Assert.That(result.ToString()).IsEqualTo("Result { Success = False, Errors = [ ErrorWithData { Type = ErrorWithData, Message = Error message number 5, Data = { Number = 5 }, Number = 5 } ]");
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