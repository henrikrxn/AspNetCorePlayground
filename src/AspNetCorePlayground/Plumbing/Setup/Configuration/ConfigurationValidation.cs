using AspNetCorePlayground.Plumbing.Configuration;
using Serilog;

namespace AspNetCorePlayground.Plumbing.Setup.Configuration;

public static class ConfigurationValidation
{
    public static WebApplicationBuilder SetupConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.SetupCorsConfigurationValidation();

        _ = builder.SetupDictionaryConfigurationValidation();

        return builder;
    }

    public static WebApplicationBuilder SetupCorsConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddOptions<CorsConfiguration>()
            .Bind(builder.Configuration.GetSection(CorsConfiguration.SectionName), options =>
            {
                Log.Information("Inside Bind method");
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static WebApplicationBuilder SetupDictionaryConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddOptions<DictionaryConfiguration>()
            .Bind(builder.Configuration.GetSection(DictionaryConfiguration.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }
}
