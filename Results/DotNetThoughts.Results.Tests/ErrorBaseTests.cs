namespace DotNetThoughts.Results.Tests;

public class ErrorBaseTests
{
    [Theory]
    [InlineData(typeof(string), "String")]
    [InlineData(typeof(List<>), "List<>")]
    [InlineData(typeof(ErrorBase), "ErrorBase")]
    [InlineData(typeof(GenericError<int>), "GenericError<Int32>")]
    [InlineData(typeof(GenericError<GenericError<string>>), "GenericError<GenericError<String>>")]
    [InlineData(typeof(TwoGenerics<int, string>), "TwoGenerics<Int32,String>")]
    [InlineData(typeof(TaskTakesTooLongError), "TaskTakesTooLongError")]
    [InlineData(typeof(TaskFailedError), "TaskFailedError")]
    public void ExpandTypeNameTest1(Type t, string result)
    {
        ErrorBase.ExpandTypeName(t).Should().Be(result);

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

    [Fact]
    public void GenericErrorContainsGenericArgumentsInType()
    {
        new GenericError<int>().Type.Should().Be("GenericError<Int32>");
        new GenericError<GenericError<string>>().Type.Should().Be("GenericError<GenericError<String>>");
    }

    [Fact]
    public void TypePropertyGetsValueFromImplementingType()
    {
        new FakeError().Type.Should().Be("FakeError");
    }

    [Fact]
    public void TypePropertyGetsValueFromImplementingType_WithDeeperInheritence()
    {
        new FakeError2().Type.Should().Be("FakeError2");
    }

    [Fact]
    public void PropertyNameAndValuesAreReturnedFromGetData()
    {
        var error = new FakeError();
        var data = error.GetData();
        data.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            { "PropertyA", "A"},
            { "PropertyB", 2}
        });
    }

    [Fact]
    public void PropertyNameAndValuesAreReturnedFromGetData_WithDeeperInheritence()
    {
        var error = new FakeError2();
        var data = error.GetData();
        data.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            { "PropertyA", "A"},
            { "PropertyB", 2},
            { "PropertyC", 3.1m}
        });
    }
}
