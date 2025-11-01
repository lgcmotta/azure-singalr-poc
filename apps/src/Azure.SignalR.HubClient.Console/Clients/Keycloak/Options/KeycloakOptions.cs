namespace Azure.SignalR.HubClient.Clients.Keycloak.Options;

public record KeycloakOptions
{
    public required string TokenUrl { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string GrantType { get; init; }

    public required string Scopes { get; init; }

    internal Dictionary<string, string> ToUrlEncodedBody()
    {
        return new Dictionary<string, string>
        {
            { "client_id", ClientId },
            { "client_secret", ClientSecret },
            { "grant_type", GrantType },
            { "scope", Scopes }
        };
    }
}