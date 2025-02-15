namespace DotNetThoughts.Results.Tests;

public class BindEachTests
{
    [Test]
    public async Task UnitResult_Errors_ShortCircuits()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(1);
        successfulResults.ShouldBe(2);
        failedResults.ShouldBe(1);
    }

    [Test]
    public async Task UnitResult_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        await Assert.That(result.Success).IsTrue();
        result.Errors.Count().ShouldBe(0);
        successfulResults.ShouldBe(6);
        failedResults.ShouldBe(0);
    }

    [Test]
    public async Task TResult_Errors_ShortCircuits()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(1);
        successfulResults.ShouldBe(2);
        failedResults.ShouldBe(1);
    }


    [Test]
    public async Task TResult_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        await Assert.That(result.Success).IsTrue();
        result.Errors.Count().ShouldBe(0);
        successfulResults.ShouldBe(6);
        failedResults.ShouldBe(0);
    }

    [Test]
    public async Task UnitResult_Errors_ShortCircuits_Tasks()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return Task.FromResult(UnitResult.Ok); };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return Task.FromResult(UnitResult.Error(new FakeError())); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = await bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.ShouldBeFalse();
        result.Errors.Count().ShouldBe(1);
        successfulResults.ShouldBe(2);
        failedResults.ShouldBe(1);
    }

    [Test]
    public async Task UnitResult_Success_Tasks()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return Task.FromResult(UnitResult.Ok); };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return Task.FromResult(UnitResult.Error(new FakeError())); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = await bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        await Assert.That(result.Success).IsTrue();
        result.Errors.Count().ShouldBe(0);
        successfulResults.ShouldBe(6);
        failedResults.ShouldBe(0);
    }
}