using Azure.SignalR.HubApi.HealthChecks;
using Azure.SignalR.HubApi.Infrastructure.Keycloak;
using Azure.SignalR.HubApi.OpenApi;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.Extensions;

internal static class ServiceCollectionsExtensions
{
    internal static IServiceCollection AddContainerHealthChecks(this IServiceCollection services)
    {
        services.AddHostedService<ReadinessBackgroundService>();
        services.AddSingleton<ReadinessHealthCheck>();

        services.AddHealthChecks()
            .AddCheck<ReadinessHealthCheck>("Readiness", tags: ["ready"]);

        return services;
    }

    internal static IServiceCollection AddAzureSignalR(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Azure:SignalR:ConnectionString");

        var signalR = services.AddSignalR();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        signalR.AddAzureSignalR();

        return services;
    }

    internal static IServiceCollection AddKeycloak(this IServiceCollection services,
        IConfiguration configuration)
    {
        var https = configuration.GetValue("Keycloak:RequireHttpsMetadata", true);

        services.AddKeycloakWebApiAuthentication(configuration, options =>
        {
            options.RequireHttpsMetadata = https;
        });

        services.AddKeycloakAuthorization(configuration);

        return services;
    }

    internal static IServiceCollection AddOpenApiDocument(this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloak = configuration.GetKeycloakOptions<KeycloakOAuth2Options>();

        var server = configuration.GetValue<string>("OpenApi:Server");

        var subscriptionRequired = configuration.GetValue("OpenApi:SubscriptionRequired", false);

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

            options.WithSubscriptionKey(subscriptionRequired);

            if (!string.IsNullOrWhiteSpace(server))
            {
                options.WithOpenApiServers(new Uri(server));
            }

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