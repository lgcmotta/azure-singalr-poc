using Azure.SignalR.HubClient.Clients.Keycloak.Responses;
using Refit;

namespace Azure.SignalR.HubClient.Clients.Keycloak;

public interface IKeycloakApi
{
    [Post("/realms/PocSignalR/protocol/openid-connect/token")]
    public Task<IApiResponse<KeycloakAuthResponse>> PostCredentialsAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        Dictionary<string, string> body,
        CancellationToken cancellationToken = default);
}