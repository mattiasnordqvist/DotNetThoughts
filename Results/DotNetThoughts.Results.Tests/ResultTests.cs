using TUnit.Assertions.AssertConditions.Throws;

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
        await Assert.That(act).ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task ValueOrThrowOnErrorResultShouldThrowExceptionWithErrorsInside()
    {
        Func<object> act = () => Result<object>.Error(new FakeError()).ValueOrThrow();
        await Assert.That(act).ThrowsExactly<ValueOrThrowException>();
    }

    [Test]
    public async Task CreateErrorResultWithoutErrorsShouldThrowException()
    {
        Action act = () => Result<object>.Error([]);
        await Assert.That(act).ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task ErrorResultWithMultipleErrorsShouldRetainAllErrors()
    {
        await Assert.That(Result<object>.Error(new FakeError(), new FakeError(), new FakeError())
            .Errors.Count()).IsEqualTo(3);
    }

    [Test]
    public async Task ErrorResultWithMultipleErrorsAsListShouldRetainAllErrors()
    {
        var result = Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError(), new FakeError() });
        await Assert.That(result.Errors.Count()).IsEqualTo(3);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult()
    {
        var firstError = new FakeError();
        var result = Result<object>.Error(new List<FakeError>() { firstError, new FakeError(), new FakeError(), new FakeError() });
        result.HasError<FakeError>(out var error);
        await Assert.That(error).IsEqualTo(firstError);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithFirstErrorWhenErrorIsPresentInResult_OutParameterVersion()
    {
        var firstError = new FakeError();
        var result = Result<object>.Error([firstError, new FakeError(), new AnotherError(), new AnotherError()]);
        var isError = result.HasError<FakeError>(out var error);
        await Assert.That(isError).IsTrue();
        await Assert.That(error).IsEqualTo(firstError);
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithNullWhenErrorIsNotPresentInResult()
    {
        await Assert.That(Result<object>.Error(new List<FakeError>() { new FakeError(), new FakeError() })
            .HasError<AnotherError>()).IsFalse();
    }

    [Test]
    public async Task HasErrorCorrectlyAnswersWithNullWhenResultIsSuccess()
    {
        var result = Result<object>.Ok(null!);
        var hasError = result.HasError<FakeError>(out var noError);
        await Assert.That(hasError).IsFalse();
        await Assert.That(noError).IsNull();
    }

    [Test]
    public async Task CastingResultToTaskOfResultRetainsResultValue()
    {
        Task<Result<object>> casted = Result<object>.Ok(1345);
        await Assert.That(casted.Result.Value).IsEqualTo(1345);
        await Assert.That(casted.Result.Success).IsTrue();
    }

    [Test]
    public async Task CastingOkResultToUnitResultReplacesValueButKeepsSuccess()
    {
        Result<Unit> casted = Result<object>.Ok(1345);
        await Assert.That(casted.Value).IsEqualTo(Unit.Instance);
        await Assert.That(casted.Success).IsTrue();
    }

    [Test]
    public async Task CastingErrorResultToUnitResultReplacesValueButKeepsErrors()
    {
        Result<Unit> casted = Result<object>.Error(new FakeError());
        await Assert.That(casted.Success).IsFalse();
        await Assert.That(casted.HasError<FakeError>()).IsNotNull();
    }

    [Test]
    public async Task ModifyingAPassedListOfErrorsDoesNotModifyResult()
    {
        var listOfErrors = new List<IError>() { new FakeError(), new FakeError(), new FakeError() };
        var result = Result<object>.Error(listOfErrors);
        listOfErrors.Add(new FakeError());
        await Assert.That(result.Errors.Count).IsEqualTo(3);
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForSuccess()
    {
        var result = Result<int>.Ok(1234);
        await Assert.That(result.ToString()).IsEqualTo("Result { Success = True, Value = 1234 }");
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForErrorWithoutData()
    {
        var result = Result<object>.Error(new FakeError());
        await Assert.That(result.ToString()).IsEqualTo("Result { Success = False, Errors = [ FakeError { Type = FakeError, Message = FakeError, Data = { } } ] }");
    }

    [Test]
    public async Task ToStringReturnsExpectedResultForErrorWithData()
    {
        var result = Result<object>.Error(new ErrorWithData(5));
        await Assert.That(result.ToString()).IsEqualTo("Result { Success = False, Errors = [ ErrorWithData { Type = ErrorWithData, Message = Error message number 5, Data = { Number = 5 }, Number = 5 } ] }");
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