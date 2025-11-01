using Azure.SignalR.HubClient.Clients.Keycloak.Responses;
using Refit;

namespace Azure.SignalR.HubClient.Clients.Keycloak;

internal interface IKeycloakApi
{
    [Post("/realms/PocSignalR/protocol/openid-connect/token")]
    internal Task<IApiResponse<KeycloakAuthResponse>> PostCredentialsAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        Dictionary<string, string> body,
        CancellationToken cancellationToken = default);
}