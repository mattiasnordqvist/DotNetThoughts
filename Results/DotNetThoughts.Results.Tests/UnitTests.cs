namespace DotNetThoughts.Results.Tests;

public class UnitTests
{
    [Test]
    public async Task UnitIsSingleInstance()
    {
        await Assert.That(Unit.Instance).IsEqualTo(Unit.Instance);
    }

    [Test]
    public async Task UnitIsSingleInstance_2()
    {
        Result<Unit> result = Result<int>.Ok(23);
        Result<Unit> result2 = Result<decimal>.Ok(65.6m);
        await Assert.That(result.Value).IsEqualTo(result2.Value);
    }
}
