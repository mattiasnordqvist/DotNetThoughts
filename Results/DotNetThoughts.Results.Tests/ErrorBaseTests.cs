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
        ErrorBase.ExpandTypeName(t).ShouldBe(result);

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
        new GenericError<int>().Type.ShouldBe("GenericError<Int32>");
        new GenericError<GenericError<string>>().Type.ShouldBe("GenericError<GenericError<String>>");
    }

    [Test]
    public async Task TypePropertyGetsValueFromImplementingType()
    {
        new FakeError().Type.ShouldBe("FakeError");
    }

    [Test]
    public async Task TypePropertyGetsValueFromImplementingType_WithDeeperInheritence()
    {
        new FakeError2().Type.ShouldBe("FakeError2");
    }

    [Test]
    public async Task PropertyNameAndValuesAreReturnedFromGetData()
    {
        var error = new FakeError();
        var data = error.GetData();
        data.ShouldBeEquivalentTo(new Dictionary<string, object?>
        {
            { "PropertyA", "A"},
            { "PropertyB", 2}
        });
    }

    [Test]
    public async Task PropertyNameAndValuesAreReturnedFromGetData_WithDeeperInheritence()
    {
        var error = new FakeError2();
        var data = error.GetData();

        data.ShouldContainKeyAndValue("PropertyA", "A");
        data.ShouldContainKeyAndValue("PropertyB", 2);
        data.ShouldContainKeyAndValue("PropertyC", 3.1m);
    }
}
