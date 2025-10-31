using Microsoft.OpenApi.Models;

namespace Azure.SignalR.HubApi.OpenApi.Security;

internal class OpenApiSecurityWrapper
{
    internal OpenApiSecurityWrapper(OpenApiSecurityScheme scheme, OpenApiSecurityRequirement requirement)
    {
        Scheme = scheme;
        Requirement = requirement;
    }
    
    public OpenApiSecurityScheme Scheme { get; }
    
    public OpenApiSecurityRequirement Requirement { get; }
}