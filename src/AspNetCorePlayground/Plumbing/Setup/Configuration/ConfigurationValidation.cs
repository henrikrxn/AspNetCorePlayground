using AspNetCorePlayground.Plumbing.Configuration;

namespace AspNetCorePlayground.Plumbing.Setup.Configuration;

public static class ConfigurationValidation
{
    public static WebApplicationBuilder SetupConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.SetupCorsConfigurationValidation();

        _ = builder.SetupDictionaryConfigurationValidation();

        _ = builder.SetupListConfigurationValidation();

        return builder;
    }

    public static WebApplicationBuilder SetupCorsConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddOptions<CorsConfiguration>()
            .BindConfiguration(CorsConfiguration.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static WebApplicationBuilder SetupDictionaryConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddOptions<DictionaryConfiguration>()
            .BindConfiguration(DictionaryConfiguration.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static WebApplicationBuilder SetupListConfigurationValidation(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddOptions<ListConfiguration>()
            .BindConfiguration(ListConfiguration.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }
}
