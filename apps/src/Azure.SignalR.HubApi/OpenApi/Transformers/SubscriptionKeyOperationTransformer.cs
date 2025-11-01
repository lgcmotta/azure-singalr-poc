using Azure.SignalR.HubApi.Mime;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi.Transformers;

internal class SubscriptionKeyOperationTransformer : IOpenApiOperationTransformer
{
    private readonly bool _required;

    public SubscriptionKeyOperationTransformer(bool required = true)
    {
        _required = required;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken _ = default)
    {
        var subscriptionParameter = new OpenApiParameter
        {
            Name = ApiManagementHeaderNames.SubscriptionKey,
            In = ParameterLocation.Header,
            Required = _required
        };

        if (operation.Parameters is not null)
        {
            operation.Parameters.Add(subscriptionParameter);

            return Task.CompletedTask;
        }

        operation.Parameters = [subscriptionParameter];

        return Task.CompletedTask;
    }
}