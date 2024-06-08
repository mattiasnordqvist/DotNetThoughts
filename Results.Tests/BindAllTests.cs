using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class BindAllTests
{

    [Fact]
    public void Unit_Errors_DoesNotShortCircuit()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, false, true, true, false });
        var result = bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
        successfulResults.Should().Be(4);
        failedResults.Should().Be(2);
    }

    [Fact]
    public void Unit_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, true, true, true, true });
        var result = bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
    }

    [Fact]
    public async Task TaskUnit_Errors_DoesNotShortCircuit()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, false, true, true, false });
        var result = await bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
        successfulResults.Should().Be(4);
        failedResults.Should().Be(2);
    }

    [Fact]
    public async Task TaskUnit_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, true, true, true, true });
        var result = await bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
    }

    [Fact]
    public void T_Errors_DoesNotShortCircuit()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, false, true, true, false });
        var result = bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(2);
        successfulResults.Should().Be(4);
        failedResults.Should().Be(2);
    }

    [Fact]
    public void T_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        var bs = Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, true, true, true, true });
        var result = bs.BindAll(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
        result.Value.Count().Should().Be(6);
    }


    [Fact]
    public async Task T_Success_WithTaskResultInput_WithTaskResultOutput()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<bool>>> success = () => { successfulResults++; return Task.FromResult(Result<bool>.Ok(true)); };
        Func<Task<Result<bool>>> failure = () => { failedResults++; return Task.FromResult(Result<bool>.Error(new FakeError())); };
        var bs = Task.FromResult(Result<IEnumerable<bool>>.Ok(new List<bool>() { true, true, true, true, true, true }));
        var result = await bs.BindAll(async x =>
        {
            if (x)
            {
                return await success();
            }
            else
            {
                return await failure();
            }
        });
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
        result.Value.Count().Should().Be(6);
    }
}
