using Azure.SignalR.HubApi.Infrastructure.Keycloak;
using Keycloak.AuthServices.Common;

namespace Azure.SignalR.HubApi.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication UsePermissiveCors(this WebApplication app)
    {
        app.Configuration.GetValue<string[]>("CorsOrigins");
        
        app.UseCors(builder => builder.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
            .WithOrigins());
        
        return app;
    }

    internal static WebApplication UseOpenApiInterface(this WebApplication app)
    {
        app.MapOpenApi("swagger/openapi/v1.json");

        var keycloak = app.Configuration.GetKeycloakOptions<KeycloakOAuth2Options>();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("openapi/v1.json", "v1");

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