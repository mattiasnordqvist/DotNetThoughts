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
        result.Value.ShouldBe(Planet.Mercury);
    }

    [Test]
    public async Task Parse_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Xena";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        result.Success.ShouldBeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().ShouldBeTrue();
    }

    [Test]
    public async Task Parse_Null_Error()
    {
        // Arrange
        // Act
        var result = EnumLoader.Parse<Planet>(null);

        // Assert
        result.Success.ShouldBeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().ShouldBeTrue();
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
        result.Value.ShouldBe(enumFormat);
    }

    [Test]
    public async Task ParseAllowNull_Null_Success()
    {
        // Arrange
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null);

        // Assert
        await Assert.That(result.Success).IsTrue();
        result.Value.ShouldBeNull();
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
        result.Value.ShouldBe(Planet.Neptune);
    }

    [Test]
    public async Task ParseAllowNull_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Murrcurry";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        result.Success.ShouldBeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().ShouldBeTrue();
    }

    [Test]
    public async Task Parse_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        result.Success.ShouldBeFalse();
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
        result.Value.ShouldBe(Planet.Mercury);
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
        result.Value.ShouldBe(Planet.Mercury);
    }

    [Test]
    public async Task ParseAllowNull_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        result.Success.ShouldBeFalse();
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
        result.Value.ShouldBe(Planet.Mercury);
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
        result.Value.ShouldBe(Planet.Mercury);
    }

    [Test]
    public async Task ParseAllowNullCaseInsensitive_null_Success()
    {
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null, true);

        // Assert
        await Assert.That(result.Success).IsTrue();
        result.Value.ShouldBe(null);
    }



}
