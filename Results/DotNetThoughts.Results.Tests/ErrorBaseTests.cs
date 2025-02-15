namespace DotNetThoughts.Results.Tests;

public class ErrorBaseTests
{
    [Test]
    [Arguments(typeof(string), "String")]
    [Arguments(typeof(List<>), "List<>")]
    [Arguments(typeof(ErrorBase), "ErrorBase")]
    [Arguments(typeof(GenericError<int>), "GenericError<Int32>")]
    [Arguments(typeof(GenericError<GenericError<string>>), "GenericError<GenericError<String>>")]
    [Arguments(typeof(TwoGenerics<int, string>), "TwoGenerics<Int32,String>")]
    [Arguments(typeof(TaskTakesTooLongError), "TaskTakesTooLongError")]
    [Arguments(typeof(TaskFailedError), "TaskFailedError")]
    public async Task ExpandTypeNameTest1(Type t, string result)
    {
        await Assert.That(ErrorBase.ExpandTypeName(t)).IsEqualTo(result);
    }

    public record TaskTakesTooLongError : ErrorBase;
    public record TaskFailedError : ErrorBase;
    public record TwoGenerics<T, T2> : ErrorBase;

    public record GenericError<T> : ErrorBase
    {
    }

    public record FakeError : ErrorBase
    {
        public string PropertyA { get; set; } = "A";
        public int PropertyB { get; set; } = 2;
    }

    public record FakeError2 : FakeError
    {
        public decimal PropertyC { get; set; } = 3.1m;
    }

    [Test]
    public async Task GenericErrorContainsGenericArgumentsInType()
    {
        await Assert.That(new GenericError<int>().Type).IsEqualTo("GenericError<Int32>");
        await Assert.That(new GenericError<GenericError<string>>().Type).IsEqualTo("GenericError<GenericError<String>>");
    }

    [Test]
    public async Task TypePropertyGetsValueFromImplementingType()
    {
        await Assert.That(new FakeError().Type).IsEqualTo("FakeError");
    }

    [Test]
    public async Task TypePropertyGetsValueFromImplementingType_WithDeeperInheritence()
    {
        await Assert.That(new FakeError2().Type).IsEqualTo("FakeError2");
    }

    [Test]
    public async Task PropertyNameAndValuesAreReturnedFromGetData()
    {
        var error = new FakeError();
        var data = error.GetData();
        var expected = new Dictionary<string, object?>
        {
            { "PropertyA", "A"},
            { "PropertyB", 2}
        };
        await Assert.That(data).IsEquivalentTo(expected);
    }

    [Test]
    public async Task PropertyNameAndValuesAreReturnedFromGetData_WithDeeperInheritence()
    {
        var error = new FakeError2();
        var data = error.GetData();

        await Assert.That(data).Contains(new KeyValuePair<string, object?>("PropertyA", "A"));
        await Assert.That(data).Contains(new KeyValuePair<string, object?>("PropertyB", 2));
        await Assert.That(data).Contains(new KeyValuePair<string, object?>("PropertyC", 3.1m));
        await Assert.That(data.Count()).IsEqualTo(3);
    }
}
