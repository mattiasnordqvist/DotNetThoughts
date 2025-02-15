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
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(1);
        await Assert.That(successfulResults).IsEqualTo(2);
        await Assert.That(failedResults).IsEqualTo(1);
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
        await Assert.That(result.Errors.Count()).IsEqualTo(0);
        await Assert.That(successfulResults).IsEqualTo(6);
        await Assert.That(failedResults).IsEqualTo(0);
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
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(1);
        await Assert.That(successfulResults).IsEqualTo(2);
        await Assert.That(failedResults).IsEqualTo(1);
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
        await Assert.That(result.Errors.Count()).IsEqualTo(0);
        await Assert.That(successfulResults).IsEqualTo(6);
        await Assert.That(failedResults).IsEqualTo(0);
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
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.Errors.Count()).IsEqualTo(1);
        await Assert.That(successfulResults).IsEqualTo(2);
        await Assert.That(failedResults).IsEqualTo(1);
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
        await Assert.That(result.Errors.Count()).IsEqualTo(0);
        await Assert.That(successfulResults).IsEqualTo(6);
        await Assert.That(failedResults).IsEqualTo(0);
    }
}