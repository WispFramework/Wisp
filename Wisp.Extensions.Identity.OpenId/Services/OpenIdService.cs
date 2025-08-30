using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Wisp.Extensions.Identity.OpenId.Config;
using Wisp.Extensions.Identity.OpenId.Data.Api;
using Wisp.Extensions.Identity.OpenId.Data.Client;
using Wisp.Framework.Extensions;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;

namespace Wisp.Extensions.Identity.OpenId.Services;

public class OpenIdService(OpenIdConnectClient client, IAuthenticator authenticator, OpenIdExtensionConfig extensionConfig)
{
    public const string OpenIdStateSessionKey = "oidc-state";
    public const string OpenIdTokenSessionKey = "oidc-token";
    
    /// <summary>
    /// Redirect the user to the OIDC authorization URL
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task GetAuthenticate(IHttpContext context)
    {
        var host = context.Request.Headers.GetOrDefaultIgnoreCaseReadonly("host");
        if (host is null)
            throw new InvalidDataException("cannot construct redirect_uri because there is not Host header");

        var result = await client.GetAuthUrlAsync(host, context.IsHttps);
                
        if (!result.Ok)
        {
            SetResponseError(context, result.Error);
            return;
        }

        if (context.Session is null) throw new Exception("session store not present");
                
        context.Session?.Remove(OpenIdStateSessionKey);
        context.Session?.Set(OpenIdStateSessionKey, result.Value);
        
        context.Response.StatusCode = 307;
        context.Response.Headers.Add("Location", result.Value.AuthUrl);
    }

    /// <summary>
    /// The OIDC authorization callback. Performs the code~token exchange and authenticated the user with
    /// any available IAuthenticator
    /// </summary>
    /// <param name="context"></param>
    public async Task GetAuthCallback(IHttpContext context)
    {
        var query = ExtractQueryParams(context);

        if (!ValidateStateAndParams(context, query, out var localState))
            return;

        var host = GetHostOrThrow(context);
        
        var tokenResult = await client.GetTokenAsync(localState!, query.Code!, host, context.IsHttps);
        if (!tokenResult.Ok)
        {
            SetResponseError(context, tokenResult.Error);
            return;
        }
        
        context.Session?.Remove(OpenIdTokenSessionKey);
        tokenResult.Value.TokenValidFrom = DateTime.UtcNow;
        context.Session?.Set(OpenIdTokenSessionKey, tokenResult.Value);

        var userInfoResult = await client.GetUserInfoAsync(tokenResult.Value);
        if (!userInfoResult.Ok)
        {
            SetResponseError(context, userInfoResult.Error);
            return;
        }
        
        var principal = BuildPrincipal(userInfoResult.Value, tokenResult.Value);
        if (!await authenticator.Authenticate(principal))
        {
            SetResponseError(context, new { Error = "unknown error while authenticating" });
            return;
        }
        
        RedirectSuccess(context);
    }

    /// <summary>
    /// Get the user's OpenID Access Token (or refresh it)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string?> GetToken(IHttpContext context)
    {
        var tokenState = context.Session?.Get<OpenIdTokenResponse>(OpenIdTokenSessionKey);
        if (tokenState is null) return null;

        if (DateTime.UtcNow.AddSeconds(tokenState.ExpiresIn) >= tokenState.TokenValidFrom)
        {
            tokenState = await RefreshToken(context);
            if(tokenState is null) return null;
        }

        return tokenState.AccessToken;
    }

    
    /// <summary>
    /// Refresh the user's access token.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>True is success, False if refresh token is expired</returns>
    private async Task<OpenIdTokenResponse?> RefreshToken(IHttpContext context)
    {
        var tokenState = context.Session?.Get<OpenIdTokenResponse>(OpenIdTokenSessionKey);
        if (tokenState is null) return null;

        if (DateTime.UtcNow.AddSeconds(tokenState.RefreshExpiresIn) >= tokenState.TokenValidFrom) return null;
        var newTokens = await client.RefreshTokenAsync(tokenState.RefreshToken);
        if (!newTokens.Ok) return null;
        var token = newTokens.Value;
        token.TokenValidFrom = DateTime.UtcNow;
        
        context.Session?.Remove(OpenIdTokenSessionKey);
        context.Session?.Set(OpenIdTokenSessionKey, token);

        return token;
    }

    private (string? State, string? SessionState, string? Iss, string? Code) ExtractQueryParams(IHttpContext context)
    {
        return (
            context.Request.QueryParams.GetOrDefaultIgnoreCaseReadonly("state")!,
            context.Request.QueryParams.GetOrDefaultIgnoreCaseReadonly("session_state")!,
            context.Request.QueryParams.GetOrDefaultIgnoreCaseReadonly("iss")!,
            context.Request.QueryParams.GetOrDefaultIgnoreCaseReadonly("code")!
        );
    }

    private bool ValidateStateAndParams(IHttpContext context,
        (string? State, string? SessionState, string? Iss, string? Code) query, out OpenIdAuthCall? localState)
    {
        localState = context.Session?.Get<OpenIdAuthCall>(OpenIdStateSessionKey);
        
        if (query.State is null || query.SessionState is null || query.Iss is null || query.Code is null || localState is null)
        {
            SetResponseError(context, new { Error = "invalid request" });
            return false;
        }

        return true;
    }

    public string GetHostOrThrow(IHttpContext context)
        => context.Request.Headers.GetOrDefaultIgnoreCaseReadonly("host") 
           ?? throw new InvalidDataException("cannot construct redirect_uri because the request is missing the Host header");

    private UserPrincipal BuildPrincipal(OpenIdUserInfo userInfo, OpenIdTokenResponse tokenResponse)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.AccessToken);
        var roles = jwt.Claims
            .Where(c => c.Type == extensionConfig.RolesClaimName)
            .Select(c => c.Value)
            .ToList();
        
        var username = extensionConfig.UsernameProp.Compile()(userInfo);
        if (username is null)
            throw new InvalidDataException("invalid user information received from OIDC provider");

        return new UserPrincipal
        {
            Username = username,
            Roles = roles
        };
    }

    private void RedirectSuccess(IHttpContext context)
    {
        context.Response.StatusCode = 302;
        context.Response.Headers.Add("Location", extensionConfig.SuccessRedirectUri);
    }

    public async Task GetLogout(IHttpContext context)
    {
        context.Session?.Remove(OpenIdStateSessionKey);
                
        await authenticator.Deauthenticate();
                
        context.Response.StatusCode = 302;
        context.Response.Headers.Add("Location", extensionConfig.SuccessRedirectUri);
    }
    
    private static void SetResponseError(IHttpContext context, object error)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var errJson = JsonSerializer.Serialize(error);
        var ms = new MemoryStream(errJson.AsUtf8Bytes());
        context.Response.Body = ms;
    }
}