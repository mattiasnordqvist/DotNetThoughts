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

    [Fact]
    public void Parse_ExistsAsGivenEnum_Success()
    {
        // Arrange
        var validPlanet = "Mercury";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Mercury);
    }

    [Fact]
    public void Parse_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Xena";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        result.Success.Should().BeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().Should().BeTrue();
    }

    [Fact]
    public void Parse_Null_Error()
    {
        // Arrange
        // Act
        var result = EnumLoader.Parse<Planet>(null);

        // Assert
        result.Success.Should().BeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().Should().BeTrue();
    }

    [Fact]
    public void Parse_ImplicitlyNumeric_Success()
    {
        // Arrange
        var enumFormat = Planet.Venus;
        var implicitlyNumericFormat = ((int)enumFormat).ToString();

        // Act
        var result = EnumLoader.Parse<Planet>(implicitlyNumericFormat);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(enumFormat);
    }

    [Fact]
    public void ParseAllowNull_Null_Success()
    {
        // Arrange
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void ParseAllowNull_NotNull_Success()
    {
        // Arrange
        var validPlanet = "Neptune";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Neptune);
    }

    [Fact]
    public void ParseAllowNull_DoesNotExistAsGivenEnum_Error()
    {
        // Arrange
        var invalidPlanet = "Murrcurry";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        result.Success.Should().BeFalse();
        result.HasError<EnumValueMustExistError<Planet>>().Should().BeTrue();
    }

    [Fact]
    public void Parse_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.Parse<Planet>(invalidPlanet);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ParseCaseInsensitive_UpperCased_Success()
    {
        // Arrange
        var validPlanet = "MERCURY";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet, true);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Mercury);
    }


    [Fact]
    public void ParseCaseInsensitive_RandomCased_Success()
    {
        // Arrange
        var validPlanet = "MeRCuRY";

        // Act
        var result = EnumLoader.Parse<Planet>(validPlanet, true);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Mercury);
    }

    [Fact]
    public void ParseAllowNull_UpperCased_Error()
    {
        // Arrange
        var invalidPlanet = "MERCURY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(invalidPlanet);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void ParseAllowNullCaseInsensitive_UpperCased_Success()
    {
        // Arrange
        var validPlanet = "MERCURY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet, true);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Mercury);
    }


    [Fact]
    public void ParseAllowNullCaseInsensitive_RandomCased_Success()
    {
        // Arrange
        var validPlanet = "MeRCuRY";

        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(validPlanet, true);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(Planet.Mercury);
    }

    [Fact]
    public void ParseAllowNullCaseInsensitive_null_Success()
    {
        // Act
        var result = EnumLoader.ParseAllowNull<Planet>(null, true);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }



}
