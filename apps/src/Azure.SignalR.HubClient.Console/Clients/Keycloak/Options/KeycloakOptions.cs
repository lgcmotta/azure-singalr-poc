namespace Azure.SignalR.HubClient.Clients.Keycloak.Options;

internal record KeycloakOptions
{
    internal required string TokenUrl { get; init; }

    internal required string ClientId { get; init; }

    internal required string ClientSecret { get; init; }

    internal required string GrantType { get; init; }

    internal required string Scopes { get; init; }

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