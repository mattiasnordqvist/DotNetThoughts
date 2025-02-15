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
        result.Value.ShouldBe((Unit.Instance, 2, "c"));
    }

    [Test]
    public async Task SelectMany_OneFailure()
    {
        var result =
            from a in UnitResult.Ok
            from b in Result<int>.Ok(2)
            from c in Result<string>.Error(new FakeError())
            select (a, b, c);
        result.Success.ShouldBeFalse();
        result.HasError<FakeError>().ShouldBeTrue();
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
        result.Success.ShouldBeFalse();
        result.HasError<FakeError>().ShouldBeTrue();
        successfulResults.ShouldBe(1);
        failedResults.ShouldBe(1);
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
        result.Value.ShouldBe((Unit.Instance, 2, "c"));
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
        result.Value.ShouldBe((Unit.Instance, 2, "c"));
    }

}
