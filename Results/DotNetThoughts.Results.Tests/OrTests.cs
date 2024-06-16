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
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void SuccessfulOrFailureEqualsFailure()
    {
        var result = UnitResult.Ok.Or(_unitResultError);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ReturnValuesShouldBeCombined()
    {
        var result = Result<int>.Ok(1).Or(Result<int>.Ok(2));
        result.Success.Should().BeTrue();
        result.Value.Should().Be((1, 2));
    }

    [Fact]
    public void ErrorsShouldBeCollectedFromAllResults()
    {
        var result = Result<int>.Error(new FakeError()).Or(_intResultError);
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
    }

    [Fact]
    public void ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = _intResultError.Or(_intResultError).Or(Result<bool>.Error(new FakeError()));
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(3);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasks()
    {
        var result = await _intResultErrorTask
            .Or(_intResultErrorTask);
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError);
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
    }

    [Fact]
    public async Task ErrorsShouldBeCollectedFromAll3ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError).Or(_intResultErrorTask);
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(3);
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
        result.Success.Should().BeTrue();
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
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(8);
    }

    [Fact]
    public void StaticOr_ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = Extensions.OrResult(
             _intResultError,
             _intResultError,
             Result<bool>.Error(new FakeError()));
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(3);
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
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(4);
        successfulResults.Should().Be(4);
        failedResults.Should().Be(4);
    }
}