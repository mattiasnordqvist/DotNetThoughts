namespace DotNetThoughts.Results.Validation.Tests;

public class EnumLoaderTests
{
    enum Planet
    {
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto
    }

    [Test]
    public async Task Parse_ExistsAsGivenEnum_Success()
    {
        // Arrange
        var validPlanet = "Mercury";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Mercury);
    }

    [Test]
    public async Task Parse_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Xena";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<EnumValueMustExistError<Planet>>()).IsTrue();
    }

    [Test]
    public async Task Parse_Null_Error()
    {
        // Arrange
        // Act
        var result = EnumLoader.Parse<Planet>(null);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<EnumValueMustExistError<Planet>>()).IsTrue();
    }

    [Test]
    public async Task Parse_ImplicitlyNumeric_Success()
    {
        // Arrange
        var enumFormat = Planet.Venus;
        var implicitlyNumericFormat = ((int)enumFormat).ToString();

        // Act
        var result = EnumLoader.Parse<Planet>(implicitlyNumericFormat);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(enumFormat);
    }

    [Test]
    public async Task ParseAllowNull_Null_Success()
    {
        // Arrange
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsNull();
    }

    [Test]
    public async Task ParseAllowNull_NotNull_Success()
    {
        // Arrange
        var validPlanet = "Neptune";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Neptune);
    }

    [Test]
    public async Task ParseAllowNull_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Murrcurry";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.HasError<EnumValueMustExistError<Planet>>()).IsTrue();
    }

    [Test]
    public async Task Parse_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task ParseCaseInsensitive_UpperCased_Success()
    {
        // Arrange
        var validPlanet = "MERCURY";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Mercury);
    }

    [Test]
    public async Task ParseCaseInsensitive_RandomCased_Success()
    {
        // Arrange
        var validPlanet = "MeRCuRY";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Mercury);
    }

    [Test]
    public async Task ParseAllowNull_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task ParseAllowNullCaseInsensitive_UpperCased_Success()
    {
        // Arrange
        var validPlanet = "MERCURY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Mercury);
    }

    [Test]
    public async Task ParseAllowNullCaseInsensitive_RandomCased_Success()
    {
        // Arrange
        var validPlanet = "MeRCuRY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsEqualTo(Planet.Mercury);
    }

    [Test]
    public async Task ParseAllowNullCaseInsensitive_null_Success()
    {
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Value).IsNull();
    }
}
