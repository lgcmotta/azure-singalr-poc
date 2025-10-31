using Azure.SignalR.HubApi.OpenApi.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi.Transformers;

internal sealed class JwtBearerSchemeTransformer : SecuritySchemeDocumentTransformer, IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var provider = context.ApplicationServices.GetService<IAuthenticationSchemeProvider>();

        if (provider is null)
        {
            return;
        }

        var schemes = await provider.GetAllSchemesAsync();

        if (schemes.All(scheme => scheme.Name != JwtBearerDefaults.AuthenticationScheme))
        {
            return;
        }

        var security = OpenApiJwtBearerSecurityFactory.Create();

        ApplySecurity(document, security);
    }
}