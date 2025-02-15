namespace DotNetThoughts.Results.Tests;

public class OrTests
{
    private static readonly Result<Unit> _unitResultError = UnitResult.Error(new FakeError());
    private static readonly Result<int> _intResultError = Result<int>.Error(new FakeError());
    private static readonly Task<Result<int>> _intResultErrorTask = Task.FromResult(Result<int>.Error(new FakeError()));

    [Test]
    public async Task SuccessfulOrSuccesfulEqualsSuccesful()
    {
        var result = UnitResult.Ok.Or(UnitResult.Ok);
        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task SuccessfulOrFailureEqualsFailure()
    {
        var result = UnitResult.Ok.Or(_unitResultError);
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task ReturnValuesShouldBeCombined()
    {
        var result = Result<int>.Ok(1).Or(Result<int>.Ok(2));
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((1, 2));
    }

    [Test]
    public async Task ErrorsShouldBeCollectedFromAllResults()
    {
        var result = Result<int>.Error(new FakeError()).Or(_intResultError);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = _intResultError.Or(_intResultError).Or(Result<bool>.Error(new FakeError()));
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(3);
    }

    [Test]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasks()
    {
        var result = await _intResultErrorTask
            .Or(_intResultErrorTask);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ErrorsShouldBeCollectedFromAll2ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task ErrorsShouldBeCollectedFromAll3ResultTasksMixedWithNoTasks()
    {
        var result = await _intResultErrorTask.Or(_intResultError).Or(_intResultErrorTask);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(3);
    }

    [Test]
    public async Task AllSuccessfulEqualsSuccesful()
    {
        var result = UnitResult.Ok
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok)
            .Or(UnitResult.Ok);
        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task AllErrorsEqualsErrors()
    {
        var result = _unitResultError
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError)
            .Or(_unitResultError);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(8);
    }

    [Test]
    public async Task StaticOr_ErrorsShouldBeCollectedFromAll3Results()
    {
        var result = Extensions.OrResult(
             _intResultError,
             _intResultError,
             Result<bool>.Error(new FakeError()));
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(3);
    }

    [Test]
    public async Task StaticOr_AllResultsAreEvaluated()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return _unitResultError; };
        var result = Extensions.OrResult(
             success(), failure(), success(), failure(),
             success(), failure(), success(), failure()
             );
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(4);
        await Assert.That(successfulResults).IsEqualTo(4);
        await Assert.That(failedResults).IsEqualTo(4);
    }
}