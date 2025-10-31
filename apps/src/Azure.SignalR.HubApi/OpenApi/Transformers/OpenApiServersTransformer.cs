using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi.Transformers;

internal sealed class OpenApiServersTransformer : IOpenApiDocumentTransformer
{
    private readonly IEnumerable<Uri> _serverUrls;

    internal OpenApiServersTransformer(params IEnumerable<Uri> serverUrls)
    {
        _serverUrls = serverUrls;
    }
    
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers ??= [];

        foreach (var serverUrl in _serverUrls.ToArray())
        {
            document.Servers.Add(new OpenApiServer
            {
                Url = serverUrl.ToString()
            });
        }

        return Task.CompletedTask;
    }
}