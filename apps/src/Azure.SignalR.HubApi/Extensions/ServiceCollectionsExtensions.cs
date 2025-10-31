using Azure.SignalR.HubApi.Infrastructure.Keycloak;
using Azure.SignalR.HubApi.OpenApi;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.Extensions;

internal static class ServiceCollectionsExtensions
{
    internal static IServiceCollection AddOpenApiDocument(this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloak = configuration.GetKeycloakOptions<KeycloakOAuth2Options>();

        services.AddOpenApi(options =>
        {
            options.WithOpenApiInfo(info =>
            {
                info.Title = configuration.GetValue<string>("OpenApi:Title");
                info.Version = "v1";
                info.Contact = new OpenApiContact
                {
                    Name = configuration.GetValue<string>("OpenApi:Contact:Name"),
                    Email = configuration.GetValue<string>("OpenApi:Contact:Email")
                };
            });

            if (keycloak is not null)
            {
                options.AddOAuth2SecurityScheme(oauth2 =>
                {
                    oauth2.WithAuthorizationUrl(keycloak.KeycloakAuthEndpoint)
                        .WithTokenUrl(keycloak.KeycloakTokenEndpoint)
                        .WithOpenIdScope()
                        .WithEmailScope()
                        .WithProfileScope();
                });
            }

            options.AddJwtBearerSecurityScheme();
        });
        
        return services;
    }

    internal static IServiceCollection AddKeycloakAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloak = configuration.GetKeycloakOptions<KeycloakOAuth2Options>();

        services.AddAuthorization()
            .AddKeycloakAuthorization(options =>
            {
                options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
                options.RolesResource = keycloak?.Resource ?? "hub_client";
            });

        return services;
    }
}