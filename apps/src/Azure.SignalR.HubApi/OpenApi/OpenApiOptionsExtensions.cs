using Azure.SignalR.HubApi.OpenApi.Security;
using Azure.SignalR.HubApi.OpenApi.Transformers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi;

internal static class OpenApiOptionsExtensions
{
    internal static OpenApiOptions AddJwtBearerSecurityScheme(this OpenApiOptions options)
    {
        return options.AddDocumentTransformer(new JwtBearerSchemeTransformer());
    }

    internal static OpenApiOptions AddOAuth2SecurityScheme(this OpenApiOptions options,
        Action<OpenApiOAuth2SecurityBuilder> configureOAuth2)
    {
        return options.AddDocumentTransformer(new OAuth2SecurityTransformer(configureOAuth2));
    }

    internal static OpenApiOptions WithOpenApiInfo(this OpenApiOptions options, Action<OpenApiInfo> configureInfo)
    {
        var info = new OpenApiInfo();

        configureInfo(info);

        return options.AddDocumentTransformer(new OpenApiInfoTransformer(info));
    }

    internal static OpenApiOptions WithOpenApiServers(this OpenApiOptions options, params IEnumerable<Uri> serverUrls)
    {
        return options.AddDocumentTransformer(new OpenApiServersTransformer(serverUrls));
    }

    internal static OpenApiOptions WithSubscriptionKey(this OpenApiOptions options, bool required = true)
    {
        return options.AddOperationTransformer(new SubscriptionKeyOperationTransformer(required));
    }
}