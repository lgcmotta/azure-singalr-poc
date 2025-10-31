using Azure.SignalR.HubApi.OpenApi.Security;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi.Transformers;

internal abstract class SecuritySchemeDocumentTransformer
{
    protected SecuritySchemeDocumentTransformer()
    { }
    
    protected virtual void ApplySecurity(OpenApiDocument document, OpenApiSecurityWrapper security)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add(security.Scheme.Reference.Id, security.Scheme);

        document.SecurityRequirements ??= [];
        document.SecurityRequirements.Add(security.Requirement);

        var operations = document.Paths
            .Select(path => path.Value)
            .SelectMany(path => path.Operations)
            .ToArray();

        foreach (var operation in operations)
        {
            operation.Value.Security.Add(security.Requirement);
        }
    }
}