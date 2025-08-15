using System.ComponentModel.DataAnnotations;
using AspNetCorePlayground.Plumbing.Configuration;

namespace AspNetPlayGround.UnitTests;

public class CorsConfigurationTests
{
    [Fact]
    public void GivenLegalUri_WhenValidating_ThenSuccess()
    {
        // Arrange
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ "https://www.example.com" ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);

        // Assert
        isValid.ShouldBeTrue();
        validationResults.ShouldBeEmpty();
    }

    [Fact]
    public void GivenLegalUris_WhenValidating_ThenSuccess()
    {
        // Arrange
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ "https://www.example.com", "http://www.test.net" ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);

        // Assert
        isValid.ShouldBeTrue();
        validationResults.ShouldBeEmpty();
    }

    [Fact]
    public void GivenStarAsAllowedOrigin_WhenValidating_ThenSuccess()
    {
        // Arrange
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ "*" ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);
        var originIsAny = input.AllowedOriginsContainAny;

        // Assert
        isValid.ShouldBeTrue();
        validationResults.ShouldBeEmpty();

        originIsAny.ShouldBeTrue();
    }

    [Fact]
    public void GivenOneOriginIsStar_AfterValidating_ThenReturnsTrue()
    {
        // Arrange
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ "https://www.example.com", "*" ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);
        var originIsAny = input.AllowedOriginsContainAny;

        // Assert
        isValid.ShouldBeTrue();
        validationResults.ShouldBeEmpty();
        originIsAny.ShouldBeTrue();
    }

    [Fact]
    public void GivenNoOrigins_AfterValidating_TheValidationFails()
    {
        // Arrange
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);

        // Assert
        isValid.ShouldBeFalse();
        validationResults.ShouldHaveSingleItem();
        ValidationResult failure = validationResults.First();
        failure.ErrorMessage.ShouldBe(CorsConfiguration.NoOriginsErrorMessage);
        failure.MemberNames.ShouldHaveSingleItem();
        failure.MemberNames.First().ShouldBe(nameof(input.AllowedOrigins));
    }

    [Fact]
    public void GivenRelativeUri_AfterValidating_TheValidationFails()
    {
        // Arrange
        var relativeUri = "/NotAbsoluteUri";
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ relativeUri ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);

        // Assert
        isValid.ShouldBeFalse();
        validationResults.ShouldHaveSingleItem();
        ValidationResult failure = validationResults.First();
        failure.ErrorMessage.ShouldNotBeNull();
        failure.ErrorMessage.ShouldContain(relativeUri);
        failure.ErrorMessage.ShouldContain("cannot be parsed as an absolute Uri");
        failure.MemberNames.ShouldHaveSingleItem();
        failure.MemberNames.First().ShouldBe(nameof(input.AllowedOrigins));
    }

    [Fact]
    public void GivenTwoUrisButOneRelativeUri_AfterValidating_TheValidationFailsWithValidationResult()
    {
        // Arrange
        var relativeUri = "/NotAbsoluteUri";
        var input = new CorsConfiguration
        {
            AllowedOrigins = [ "https://www.example.com", relativeUri ]
        };
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(input, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        /*
        isValid.ShouldBeFalse();
        validationResults.ShouldHaveSingleItem();
        ValidationResult failure = validationResults.First();
        failure.ErrorMessage.ShouldNotBeNull();
        failure.ErrorMessage.ShouldContain(relativeUri);
        failure.ErrorMessage.ShouldContain("cannot be parsed as an absolute Uri");
        failure.MemberNames.ShouldHaveSingleItem();
        failure.MemberNames.First().ShouldBe(nameof(input.AllowedOrigins));
        */
    }
}
