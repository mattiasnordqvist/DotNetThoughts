namespace DotNetThoughts.Results.Validation.Tests;

public class GeneralValidationTests
{

    [Test]
    public async Task Parse_Parseable_Success()
    {
        // Arrange
        var parseable = "4000";
        // Act
        var parseResult = GeneralValidation.Parse<long, string?>(parseable, StringToLong);
        // Assert
        parseawait Assert.That(result.Success).IsTrue();
        parseResult.Value.ShouldBe(long.Parse(parseable));
    }

    [Test]
    public async Task Parse_NotParseable_Error()
    {
        // Arrange
        var unparseable = "fyratusen";
        // Act
        var parseResult = GeneralValidation.Parse<long, string?>(unparseable, StringToLong);
        // Assert
        parseResult.Success.ShouldBeFalse();
        parseResult.HasError<UnparseableError>().ShouldBeTrue();
    }

    [Test]
    public async Task Parse_Null_Error()
    {
        // Arrange
        // Act
        var parseResult = GeneralValidation.Parse<long, string?>((string?)null, StringToLong);
        // Assert
        parseResult.Success.ShouldBeFalse();
        parseResult.HasError<MissingArgumentError>().ShouldBeTrue();
    }

    [Test]
    public async Task ParseAllowNull_Null_Success()
    {
        // Arrange
        // Act
        var parseResult = GeneralValidation.ParseAllowNull((string?)null, StringToValueObject);
        // Assert
        parseawait Assert.That(result.Success).IsTrue();
        parseResult.Value.ShouldBe(null);
    }

    [Test]
    public async Task ParseAllowNull_NotNullParseable_Success()
    {
        // Arrange
        var parseable = "10";
        // Act
        var parseResult = GeneralValidation.ParseAllowNull(parseable, StringToValueObject);
        // Assert
        parseawait Assert.That(result.Success).IsTrue();
        parseResult.Value.ShouldBe(new SomeValueObject(long.Parse(parseable)));
    }

    [Test]
    public async Task ParseAllowNull_NotNullNotParseable_Error()
    {
        // Arrange
        var parseable = "tio";
        // Act
        var parseResult = GeneralValidation.ParseAllowNull(parseable, StringToValueObject);
        // Assert
        parseResult.Success.ShouldBeFalse();
        parseResult.HasError<UnparseableError>().ShouldBeTrue();
    }

    [Test]
    public async Task ParseAllowNull_NullableStructs()
    {
        // Arrange
        string? parseable = "2022-12-01";
        // Act
        var parseResult = GeneralValidation.ParseAllowNullStruct(parseable, v => DateOnly.TryParse(v, out var result) ? result.Return() : Result<DateOnly>.Error(new InvalidDateError()));
        // Assert
        parseawait Assert.That(result.Success).IsTrue();
        parseResult.Value.ShouldBe(new DateOnly(2022, 12, 1));
    }

    [Test]
    public async Task ParseAllowNull_NullableStructs2()
    {
        // Arrange
        string? parseable = null;
        // Act
        var parseResult = GeneralValidation.ParseAllowNullStruct(parseable, v => DateOnly.TryParse(v, out var result) ? result.Return() : Result<DateOnly>.Error(new InvalidDateError()));
        // Assert
        parseawait Assert.That(result.Success).IsTrue();
        parseResult.Value.ShouldBe(null);
    }
    public record UnparseableError(string? Candidate) : ErrorBase;
    public static Result<long> StringToLong(string? candidate) =>
      long.TryParse(candidate, out var longified)
          ? longified.Return()
          : Result<long>.Error(new UnparseableError(candidate));
    public record class SomeValueObject(long Value);
    public static Result<SomeValueObject> StringToValueObject(string? candidate)
    {
        var fitsValueDataType = StringToLong(candidate);
        return fitsValueDataType.Success
            ? new SomeValueObject(fitsValueDataType.Value).Return()
            : Result<SomeValueObject>.Error(new UnparseableError(candidate));
    }
}
