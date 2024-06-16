using FluentAssertions;

using Xunit;

namespace DotNetThoughts.Results.Tests;

public class BindEachTests
{
    [Fact]
    public void UnitResult_Errors_ShortCircuits()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(1);
        successfulResults.Should().Be(2);
        failedResults.Should().Be(1);
    }

    [Fact]
    public void UnitResult_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<Unit>> success = () => { successfulResults++; return UnitResult.Ok; };
        Func<Result<Unit>> failure = () => { failedResults++; return UnitResult.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
    }

    [Fact]
    public void TResult_Errors_ShortCircuits()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(1);
        successfulResults.Should().Be(2);
        failedResults.Should().Be(1);
    }


    [Fact]
    public void TResult_Success()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Result<bool>> success = () => { successfulResults++; return Result<bool>.Ok(true); };
        Func<Result<bool>> failure = () => { failedResults++; return Result<bool>.Error(new FakeError()); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
    }

    [Fact]
    public async Task UnitResult_Errors_ShortCircuits_Tasks()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return Task.FromResult(UnitResult.Ok); };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return Task.FromResult(UnitResult.Error(new FakeError())); };
        List<bool> bs = new List<bool>() { true, true, false, true, true, false };
        var result = await bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeFalse();
        result.Errors.Count().Should().Be(1);
        successfulResults.Should().Be(2);
        failedResults.Should().Be(1);
    }

    [Fact]
    public async Task UnitResult_Success_Tasks()
    {
        int successfulResults = 0;
        int failedResults = 0;
        Func<Task<Result<Unit>>> success = () => { successfulResults++; return Task.FromResult(UnitResult.Ok); };
        Func<Task<Result<Unit>>> failure = () => { failedResults++; return Task.FromResult(UnitResult.Error(new FakeError())); };
        List<bool> bs = new List<bool>() { true, true, true, true, true, true };
        var result = await bs.Return<IEnumerable<bool>>().BindEach(x => x ? success() : failure());
        result.Success.Should().BeTrue();
        result.Errors.Count().Should().Be(0);
        successfulResults.Should().Be(6);
        failedResults.Should().Be(0);
    }
}