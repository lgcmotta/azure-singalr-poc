using Azure.SignalR.HubApi.Infrastructure.Keycloak;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net.Mime;
using System.Text.Json;

namespace Azure.SignalR.HubApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication MapReadinessHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        return app;
    }


    internal static WebApplication MapLivenessHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/healthz/live", new HealthCheckOptions
        {
            Predicate = check => !check.Tags.Contains("ready"),
            ResponseWriter = async (context, report) =>
            {
                var components = report.Entries.ToDictionary(
                    entry => JsonNamingPolicy.SnakeCaseLower.ConvertName(entry.Key),
                    pair => pair.Value.Status.ToString()
                );

                var (statusCode, contentType) = report.Status switch
                {
                    HealthStatus.Healthy => (StatusCodes.Status200OK, MediaTypeNames.Application.Json),
                    HealthStatus.Degraded => (StatusCodes.Status200OK, MediaTypeNames.Application.ProblemJson),
                    _ => (StatusCodes.Status503ServiceUnavailable, MediaTypeNames.Application.ProblemJson)
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = contentType;

                await context.Response.WriteAsJsonAsync(new
                {
                    Status = report.Status.ToString(),
                    Components = components
                });
            }
        });

        return app;
    }

    internal static WebApplication UsePermissiveCors(this WebApplication app)
    {
        var origins = app.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? [];

        app.UseCors(builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
            .WithOrigins(origins)
        );

        return app;
    }

    internal static WebApplication UseOpenApiInterface(this WebApplication app)
    {
        var pattern = app.Configuration.GetValue<string>("OpenApi:DocumentEndpoint", "swagger/openapi/v1.json");

        var swaggerEndpoint = app.Configuration.GetValue<string>("OpenApi:SwaggerEndpoint", "openapi/v1.json");

        app.MapOpenApi(pattern);

        var keycloak = app.Configuration.GetKeycloakOptions<KeycloakOAuth2Options>();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(swaggerEndpoint, "v1");

            if (keycloak is null)
            {
                return;
            }

            options.OAuthClientId(keycloak.Resource);
            options.OAuthUsePkce();
            options.OAuthScopes(keycloak.Scopes);
        });

        return app;
    }
}