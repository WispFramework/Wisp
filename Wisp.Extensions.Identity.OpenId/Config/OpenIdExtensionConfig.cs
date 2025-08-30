using System.Linq.Expressions;
using Wisp.Extensions.Identity.OpenId.Data.Api;

namespace Wisp.Extensions.Identity.OpenId.Config;

public class OpenIdExtensionConfig
{
    /// <summary>
    /// The user info username property
    /// </summary>
    public Expression<Func<OpenIdUserInfo, string?>> UsernameProp { get; set; } 
        = info => info.PreferredUsername; 

    /// <summary>
    /// The name of the roles claim
    /// </summary>
    public string RolesClaimName { get; set; } = "roles";

    /// <summary>
    /// The success redirect URI
    /// </summary>
    public string SuccessRedirectUri { get; set; } = "/";
    
    /// <summary>
    /// The error redirect URI
    /// </summary>
    public string ErrorRedirectUri { get; set; } = "/";
    
    /// <summary>
    /// The URI of the authentication endpoint
    /// </summary>
    public string AuthUrl { get; set; } = "/auth/oidc/authenticate";

    /// <summary>
    /// The URI of the OIDC callback endpoint
    /// </summary>
    public string CallbackUrl { get; set; } = "/auth/oidc/callback";
    
    /// <summary>
    /// The URI of the logout endpoint
    /// </summary>
    public string LogoutUrl { get; set; } = "/auth/oidc/logout";
    
    /// <summary>
    /// URL Of the OpenID Discovery Document
    /// </summary>
    public string DiscoveryUrl { get; set; } = "";
    
    /// <summary>
    /// The public Client ID
    /// </summary>
    public string ClientId { get; set; } = "";
    
    
    /// <summary>
    /// The private Client Secret
    /// </summary>
    public string ClientSecret { get; set; } = "";
    
    /// <summary>
    /// The OpenID scopes, separated with space
    /// </summary>
    public string Scopes { get; set; } = "";
}