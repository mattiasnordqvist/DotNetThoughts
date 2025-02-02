namespace DotNetThoughts.Results.Tests;

public class OrTests
{
    private static readonly Result<Unit> _unitResultError = UnitResult.Error(new FakeError());
    private static readonly Result<int> _intResultError = Result<int>.Error(new FakeError());
    private static readonly Task<Result<int>> _intResultErrorTask = Task.FromResult(Result<int>.Error(new FakeError()));

    [Fact]
    public void SuccessfulOrSuccesfulEqualsSuccesful()
    {
        var result = UnitResult.Ok.Or(UnitResult.Ok);
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void SuccessfulOrFailureEqualsFailure()
    {
        var result = UnitResult.Ok.Or(_unitResultError);
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public void ReturnValuesShouldBeCombined()
    {
        var result = Result<int>.Ok(1).Or(Result<int>.Ok(2));
        result.Success.ShouldBeTrue();
        result.Value.ShouldBe((1, 2));
    }

    [Fact]
    public void ErrorsShouldBeCollectedFromAllResults()
    {
        var result = Result<int>.Error(new FakeError()).Or(_intResultError);
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(2);
    }

    [Fact]
    public void ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = _intResultError.Or(_intResultError).Or(Result<bool>.Error(new FakeError()));
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(3);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasks()
    {
        var result = await _intResultErrorTask
            .Or(_intResultErrorTask);
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(2);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError);
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(2);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll3ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError).Or(_intResultErrorTask);
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(3);
    }

    [Fact]
    public void AllSuccessfulEqualsSuccesful()
    {
        var result = UnitResult.Ok
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok);
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void AllErrorsEqualsErrors()
    {
        var result = _unitResultError
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError);
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(8);
    }

    [Fact]
    public void StaticOr_ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = Extensions.OrResult(
             _intResultError,
             _intResultError,
             Result<bool>.Error(new FakeError()));
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(3);
    }

    [Fact]
    public void StaticOr_AllResultsAreEvaluated()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return _unitResultError; };
        var result = Extensions.OrResult(
             success(), failure(), success(), failure(),
             success(), failure(), success(), failure()
             );
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(4);
        successfulResults.ShouldBe(4);
        failedResults.ShouldBe(4);
    }
}