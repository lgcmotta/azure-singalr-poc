using JetBrains.Annotations;
using System.Text.Json.Serialization;

namespace Azure.SignalR.HubClient.Clients.Keycloak.Responses;

[UsedImplicitly]
internal record KeycloakAuthResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; init; } = string.Empty;
}