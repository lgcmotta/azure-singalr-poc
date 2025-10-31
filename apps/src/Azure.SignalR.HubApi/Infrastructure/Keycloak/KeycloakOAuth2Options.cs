using Keycloak.AuthServices.Common;

namespace Azure.SignalR.HubApi.Infrastructure.Keycloak;

// ReSharper disable once ClassNeverInstantiated.Global
internal class KeycloakOAuth2Options : KeycloakInstallationOptions
{
    public string KeycloakAuthEndpoint => !string.IsNullOrWhiteSpace(KeycloakUrlRealm)
        ? $"{KeycloakUrlRealm}protocol/openid-connect/auth"
        : string.Empty;

    [ConfigurationKeyName("Scopes")] 
    public string[] Scopes { get; set; } = [];
}