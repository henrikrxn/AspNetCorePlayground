using System.ComponentModel.DataAnnotations;

namespace AspNetCorePlayground.Plumbing.Configuration;

public class CorsConfiguration : IValidatableObject
{
    public const string SectionName = "Cors";
    public const string CorsAllowedOrigins = $"{SectionName}:AllowedOrigins";

    public const string NoOriginsErrorMessage = $"The '{nameof(AllowedOrigins)}' list must contain at least one entry.";

    public IList<string> AllowedOrigins { get; init; } = new List<string>();

    public bool AllowedOriginsContainAny => AllowedOrigins.Contains("*");

    // This class uses System.ComponentModel.DataAnnotations.IValidatableObject for custom validation
    // The alternative is Microsoft.Extensions.Options.IValidateOptions<T>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AllowedOrigins.Count == 0)
        {
            yield return new ValidationResult(NoOriginsErrorMessage, [ nameof(AllowedOrigins) ]);
            yield break;
        }

        if (AllowedOriginsContainAny)
        {
            yield break;
        }

        foreach (var origin in AllowedOrigins)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out _))
            {
                yield return new ValidationResult(errorMessage: $"Origin '{origin}' in configuration path '{CorsAllowedOrigins}' cannot be parsed as an absolute Uri",
                                                  memberNames:[ nameof(AllowedOrigins) ]);
            }
        }
    }
}
