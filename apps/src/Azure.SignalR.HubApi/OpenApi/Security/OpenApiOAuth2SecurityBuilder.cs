using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace Azure.SignalR.HubApi.OpenApi.Security;

public class OpenApiOAuth2SecurityBuilder
{
    public const string DefaultOpenApiDescription = "Keycloak + SAML Broker (Authorization Code with PKCE)";
    public const string DefaultOpenApiReferenceId = "OAuth2";

    private readonly Dictionary<string, string> _scopes = [];
    private readonly OpenApiSecurityRequirement _requirement = [];
    private readonly OpenApiSecurityScheme _scheme = new()
    {
        Type = SecuritySchemeType.OAuth2,
        Description = DefaultOpenApiDescription,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = DefaultOpenApiReferenceId
        },
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow()
        },
    };
    
    public OpenApiOAuth2SecurityBuilder WithSchemeDescription(string description = DefaultOpenApiDescription)
    {
        if (string.IsNullOrWhiteSpace(description)) return this;

        _scheme.Description = description;

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithAuthorizationUrl([StringSyntax("Uri", "uriKind")] string authorizationUrl)
    {
        if (!Uri.IsWellFormedUriString(authorizationUrl, UriKind.Absolute))
        {
            throw new UriFormatException("Authorization URL must be a valid absolute URL");
        }

        return WithAuthorizationUrl(new Uri(authorizationUrl));
    }
    
    public OpenApiOAuth2SecurityBuilder WithAuthorizationUrl(Uri authorizationUrl)
    {
        _scheme.Flows.AuthorizationCode.AuthorizationUrl = authorizationUrl;

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithTokenUrl([StringSyntax("Uri", "uriKind")] string tokenUrl)
    {
        if (!Uri.IsWellFormedUriString(tokenUrl, UriKind.Absolute))
        {
            throw new UriFormatException("Token URL must be a valid absolute URL");
        }

        return WithTokenUrl(new Uri(tokenUrl));
    }

    /// <summary>
    /// Sets the token URL to be used for OAuth2 flow.
    /// </summary>
    /// <param name="tokenUrl">
    /// The token URL to use with OAuth2 flow. This is REQUIRED.
    /// If not present throws <see cref="InvalidOperationException"/> when <see cref="Build"/> is invoked internally.
    /// </param>
    /// <returns>
    /// The <see cref="OpenApiOAuth2SecurityBuilder"/> that can be used for further customization
    /// </returns>
    public OpenApiOAuth2SecurityBuilder WithTokenUrl(Uri tokenUrl)
    {
        _scheme.Flows.AuthorizationCode.TokenUrl = tokenUrl;

        return this;
    }


    public OpenApiOAuth2SecurityBuilder WithOpenIdScope(string? scopeDescription = null)
    {
        var description = !string.IsNullOrWhiteSpace(scopeDescription)
            ? scopeDescription
            : "Grants basic OpenID claims";

        _scopes.TryAdd("openid", description);

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithProfileScope(string? scopeDescription = null)
    {
        var description = !string.IsNullOrWhiteSpace(scopeDescription)
            ? scopeDescription
            : "Grants access to basic user profile information";

        _scopes.TryAdd("profile", description);

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithEmailScope(string? scopeDescription = null)
    {
        var description = !string.IsNullOrWhiteSpace(scopeDescription)
            ? scopeDescription
            : "Grants access to the user’s email address";

        _scopes.TryAdd("email", description);

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithCustomScope(string scope, string scopeDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentException.ThrowIfNullOrWhiteSpace(scopeDescription);

        _scopes.TryAdd(scope, scopeDescription);

        return this;
    }
    
    public OpenApiOAuth2SecurityBuilder WithSchemeReferenceId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        _scheme.Reference.Id = id;

        return this;
    }
    
    internal OpenApiSecurityWrapper Build()
    {
        if (_scheme.Flows.AuthorizationCode.AuthorizationUrl is null)
        {
            throw new InvalidOperationException("The authorization URL is required to use OAuth2 flow.");
        }

        if (_scheme.Flows.AuthorizationCode.TokenUrl is null)
        {
            throw new InvalidOperationException("The token URL is required to use OAuth2 flow.");
        }

        _scheme.Flows.AuthorizationCode.Scopes = _scopes;

        _requirement.Add(_scheme, [.. _scopes.Keys]);

        return new OpenApiSecurityWrapper(_scheme, _requirement);
    }
}