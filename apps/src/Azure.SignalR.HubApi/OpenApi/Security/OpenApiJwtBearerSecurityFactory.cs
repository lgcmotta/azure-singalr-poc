using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Azure.SignalR.HubApi.OpenApi.Security;

internal static class OpenApiJwtBearerSecurityFactory
{
    internal const string DefaultOpenApiDescription = "Paste only the JWT, Bearer prefix is already added";

    internal static OpenApiSecurityWrapper Create(
        string? openApiReferenceId = JwtBearerDefaults.AuthenticationScheme,
        string? description = DefaultOpenApiDescription)
    {
        if (string.IsNullOrWhiteSpace(openApiReferenceId))
        {
            openApiReferenceId = JwtBearerDefaults.AuthenticationScheme;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            description = DefaultOpenApiDescription;
        }

        var scheme = new OpenApiSecurityScheme
        {
            Name = HeaderNames.Authorization,
            Description = description,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = JwtConstants.HeaderType,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = openApiReferenceId
            }
        };

        var requirement = new OpenApiSecurityRequirement
        {
            { scheme, [] }
        };

        return new OpenApiSecurityWrapper(scheme, requirement);
    }
}