namespace DotNetThoughts.Results.Tests;

public class SelectManyTests
{
    [Test]
    public async Task SelectMany_ThreeSuccessful()
    {
        var result =
            from a in UnitResult.Ok
            from b in Result<int>.Ok(2)
            from c in Result<string>.Ok("c")
            select (a, b, c);
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((Unit.Instance, 2, "c"));
    }

    [Test]
    public async Task SelectMany_OneFailure()
    {
        var result =
            from a in UnitResult.Ok
            from b in Result<int>.Ok(2)
            from c in Result<string>.Error(new FakeError())
            select (a, b, c);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<FakeError>()).IsTrue();
    }

    [Test]
    public async Task SelectMany_FirstFailureShortCircuits()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };

        var result =
            from a in success()
            from b in failure()
            from c in success()
            select (a, b, c);
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<FakeError>()).IsTrue();
        await Assert.That(successfulResults).IsEqualTo(1);
        await Assert.That(failedResults).IsEqualTo(1);
    }

    [Test]
    public async Task SelectManyTasks_ThreeSuccessful()
    {
        var result = await
            (from a in Task.FromResult(UnitResult.Ok)
             from b in Result<int>.Ok(2)
             from c in Task.FromResult(Result<string>.Ok("c"))
             select (a, b, c));
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((Unit.Instance, 2, "c"));
    }

    [Test]
    public async Task SelectManyTasks2_ThreeSuccessful()
    {
        var result = await
            (from a in UnitResult.Ok
             from b in Task.FromResult(Result<int>.Ok(2))
             from c in Task.FromResult(Result<string>.Ok("c"))
             select (a, b, c));
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo((Unit.Instance, 2, "c"));
    }
}
