using AspNetCorePlayground.Plumbing.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace AspNetCorePlayground.Plumbing.Setup.Cors;

public static class RegisterCors
{
    public static WebApplication Cors(this WebApplication app)
    {
        app.UseCors(corsPolicyBuilder =>
        {
            // TODO Can this be moved back up in 'ConfigureServices' after .NET 6 wih the new cool way of having "things" ready earlier ?
            corsPolicyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod(); // TODO look into narrowing this

            Log.Information("Inside UseCors extension method");

            CorsConfiguration corsConfiguration = app.Services.GetRequiredService<IOptions<CorsConfiguration>>().Value;

            if (corsConfiguration.AllowedOriginsContainAny)
            {
                corsPolicyBuilder.AllowAnyOrigin();
            }
            else
            {
                corsPolicyBuilder.WithOrigins(corsConfiguration.AllowedOrigins.ToArray());
            }
        });

        return app;
    }
}
