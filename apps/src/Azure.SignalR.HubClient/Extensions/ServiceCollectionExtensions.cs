using Azure.SignalR.HubClient.Clients.Keycloak;
using Azure.SignalR.HubClient.Clients.Keycloak.Options;
using Refit;

namespace Azure.SignalR.HubClient.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddKeycloakClient(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Keycloak");

        services.AddOptions<KeycloakOptions>()
            .Bind(section);

        services.AddRefitClient<IKeycloakApi>()
            .ConfigureHttpClient(client =>
            {
                var baseUrl = configuration.GetValue<string>("Keycloak:BaseUrl");
                
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }
            });
        
        return services;
    }
}