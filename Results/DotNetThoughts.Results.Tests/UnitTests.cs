namespace DotNetThoughts.Results.Tests;

public class UnitTests
{
    [Test]
    public async Task UnitIsSingleInstance()
    {
        Unit.Instance.ShouldBeSameAs(Unit.Instance);
    }

    [Test]
    public async Task UnitIsSingleInstance_2()
    {
        Result<Unit> result = Result<int>.Ok(23);
        Result<Unit> result2 = Result<decimal>.Ok(65.6m);
        result.Value.ShouldBeSameAs(result2.Value);
    }
}
