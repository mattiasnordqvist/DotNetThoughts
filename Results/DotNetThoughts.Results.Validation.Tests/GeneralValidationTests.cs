namespace DotNetThoughts.Results.Validation.Tests;

public class GeneralValidationTests
{
    [Test]
    public async Task Parse_Parseable_Success()
    {
        // Arrange
        var parseable = "4000";
        // Act
        var result = GeneralValidation.Parse<long, string?>(parseable, StringToLong);
        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(long.Parse(parseable));
    }

    [Test]
    public async Task Parse_NotParseable_Error()
    {
        // Arrange
        var unparseable = "fyratusen";
        // Act
        var result = GeneralValidation.Parse<long, string?>(unparseable, StringToLong);
        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<UnparseableError>()).IsTrue();
    }

    [Test]
    public async Task Parse_Null_Error()
    {
        // Arrange
        // Act
        var result = GeneralValidation.Parse<long, string?>((string?)null, StringToLong);
        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<MissingArgumentError>()).IsTrue();
    }

    [Test]
    public async Task ParseAllowNull_Null_Success()
    {
        // Arrange
        // Act
        var result = GeneralValidation.ParseAllowNull((string?)null, StringToValueObject);
        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsNull();
    }

    [Test]
    public async Task ParseAllowNull_NotNullParseable_Success()
    {
        // Arrange
        var parseable = "10";
        // Act
        var result = GeneralValidation.ParseAllowNull(parseable, StringToValueObject);
        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new SomeValueObject(long.Parse(parseable)));
    }

    [Test]
    public async Task ParseAllowNull_NotNullNotParseable_Error()
    {
        // Arrange
        var parseable = "tio";
        // Act
        var result = GeneralValidation.ParseAllowNull(parseable, StringToValueObject);
        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<UnparseableError>()).IsTrue();
    }

    [Test]
    public async Task ParseAllowNull_NullableStructs()
    {
        // Arrange
        string? parseable = "2022-12-01";
        // Act
        var result = GeneralValidation.ParseAllowNullStruct(parseable, v => DateOnly.TryParse(v, out var result) ? result.Return() : Result<DateOnly>.Error(new InvalidDateError()));
        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(new DateOnly(2022, 12, 1));
    }

    [Test]
    public async Task ParseAllowNull_NullableStructs2()
    {
        // Arrange
        string? parseable = null;
        // Act
        var result = GeneralValidation.ParseAllowNullStruct(parseable, v => DateOnly.TryParse(v, out var result) ? result.Return() : Result<DateOnly>.Error(new InvalidDateError()));
        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsNull();
    }

    public record UnparseableError(string? Candidate) : Error;
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
