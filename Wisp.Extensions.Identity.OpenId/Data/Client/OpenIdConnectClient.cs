using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Wisp.Extensions.Identity.OpenId.Config;
using Wisp.Extensions.Identity.OpenId.Data.Api;
using Wisp.Framework.Extensions;
using Wisp.Framework.Util;
using Random = Wisp.Framework.Util.Random;

namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public class OpenIdConnectClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenIdExtensionConfig _extensionConfig;

    public OpenIdConnectClient(OpenIdExtensionConfig extensionConfig, HttpClient? httpClient = null)
    {
        _extensionConfig = extensionConfig;
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Retrieve and parse the discovery document.
    /// A status code != 0 indicates an error
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>(discovery document, status code, optional error)</returns>
    public async Task<Result<OpenIdDiscovery, OpenIdError>> GetDiscoveryAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(_extensionConfig.DiscoveryUrl, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {

            return Result<OpenIdDiscovery, OpenIdError>.Failure(new OpenIdError
            {
                ErrorType = ErrorType.HttpError,
                ErrorMessage = body,
                HttpStatusCode = (int)response.StatusCode
            });
        }
        
        var parsed = JsonSerializer.Deserialize<OpenIdDiscovery>(body);
        if (parsed is null)
        {
            return Result<OpenIdDiscovery, OpenIdError>.Failure(new OpenIdError
            {
                ErrorType = ErrorType.JsonParseError,
                ErrorMessage = "could not parse discovery document JSON",
                Context = body
            });
        }

        return Result<OpenIdDiscovery, OpenIdError>.Success(parsed);
    }

    
    /// <summary>
    /// Generate an OIDC auth URL, including a PKCS challenge and a nonce
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<OpenIdAuthCall, OpenIdError>> GetAuthUrlAsync(
        string host,
        bool https,
        CancellationToken cancellationToken = default)
    {
        
        var discoResult = await GetDiscoveryAsync(cancellationToken);
        if (!discoResult.Ok) return Result<OpenIdAuthCall, OpenIdError>.Failure(discoResult.Error);
        
        var verifier = Random.RandomString(64);
        var nonce = Random.RandomString(64);

        using var sha256 = SHA256.Create();
        byte[] challengeBytes = sha256.ComputeHash(verifier.AsUtf8Bytes());
        string challengeHash = challengeBytes.AsBase64Url();

        var url = $"{discoResult.Value.AuthorizationEndpoint}?response_type=code&client_id={_extensionConfig.ClientId}&redirect_uri={(https? "https" : "http")}://{host}{_extensionConfig.CallbackUrl}&scope=openid%20profile%20email&state={nonce}&code_challenge={challengeHash}&code_challenge_method=S256";

        var ret = new OpenIdAuthCall
        {
            AuthUrl = url,
            Verifier = verifier,
            VerifierHash = challengeHash,
            Nonce = nonce
        };
        
        return Result<OpenIdAuthCall, OpenIdError>.Success(ret);
    }

    /// <summary>
    /// Exchange a code for a token
    /// </summary>
    /// <param name="originalAuthCall"></param>
    /// <param name="code"></param>
    /// <param name="host"></param>
    /// <param name="https"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<OpenIdTokenResponse, OpenIdError>> GetTokenAsync(OpenIdAuthCall originalAuthCall,
        string code,
        string host,
        bool https,
        CancellationToken cancellationToken = default)
    {
        var discoResult = await GetDiscoveryAsync(cancellationToken);
        if (!discoResult.Ok) return Result<OpenIdTokenResponse, OpenIdError>.Failure(discoResult.Error);
        
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = $"{(https? "https" : "http")}://{host}{_extensionConfig.CallbackUrl}",
            ["client_id"] = _extensionConfig.ClientId,
            ["client_secret"] = _extensionConfig.ClientSecret,
            ["code_verifier"] = originalAuthCall.Verifier 
        };
        
        var content = new FormUrlEncodedContent(values);
        
        var response = await _httpClient.PostAsync(discoResult.Value.TokenEndpoint, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var parsed = JsonSerializer.Deserialize<OpenIdTokenResponse>(body);
            if (parsed is null) return Result<OpenIdTokenResponse, OpenIdError>.Failure(new OpenIdError {
                ErrorType = ErrorType.JsonParseError,
                ErrorMessage = "could not parse response JSON",
                Context = body
            });

            return Result<OpenIdTokenResponse, OpenIdError>.Success(parsed);
        }
        
        return Result<OpenIdTokenResponse, OpenIdError>.Failure(new OpenIdError
        {
            ErrorType = ErrorType.HttpError,
            ErrorMessage = body,
            HttpStatusCode = (int)response.StatusCode
        });
    }

    
    /// <summary>
    /// Get the User Info
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<OpenIdUserInfo, OpenIdError>> GetUserInfoAsync(OpenIdTokenResponse token,
        CancellationToken cancellationToken = default)
    {
        var discoResult = await GetDiscoveryAsync(cancellationToken);
        if (!discoResult.Ok) return Result<OpenIdUserInfo, OpenIdError>.Failure(discoResult.Error);

        var request = new HttpRequestMessage(HttpMethod.Post, discoResult.Value.UserinfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        request.Content = null;
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result<OpenIdUserInfo, OpenIdError>.Failure(new OpenIdError
            {
                ErrorType = ErrorType.HttpError,
                ErrorMessage = body,
                HttpStatusCode = (int)response.StatusCode
            });
        }
        
        var parsed = JsonSerializer.Deserialize<OpenIdUserInfo>(body);
        if (parsed is null)
        {
            if (parsed is null) return Result<OpenIdUserInfo, OpenIdError>.Failure(new OpenIdError {
                ErrorType = ErrorType.JsonParseError,
                ErrorMessage = "could not parse response JSON",
                Context = body
            });
        }
        
        return Result<OpenIdUserInfo, OpenIdError>.Success(parsed);
    }

    /// <summary>
    /// Refresh the access token.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<OpenIdTokenResponse, OpenIdError>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var discoResult = await GetDiscoveryAsync(cancellationToken);
        if (!discoResult.Ok) return Result<OpenIdTokenResponse, OpenIdError>.Failure(discoResult.Error);
        
        var values = new Dictionary<string, string>()
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _extensionConfig.ClientId,
            ["client_secret"] = _extensionConfig.ClientSecret,
            ["refresh_token"] = refreshToken
        };
        
        var content = new FormUrlEncodedContent(values);
        var response = await _httpClient.PostAsync(discoResult.Value.TokenEndpoint, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result<OpenIdTokenResponse, OpenIdError>.Failure(new OpenIdError
            {
                ErrorType = ErrorType.HttpError,
                ErrorMessage = body,
                HttpStatusCode = (int)response.StatusCode
            });
        }
        
        var parsed = JsonSerializer.Deserialize<OpenIdTokenResponse>(body);
        if (parsed is null) return Result<OpenIdTokenResponse, OpenIdError>.Failure(new OpenIdError {
            ErrorType = ErrorType.JsonParseError,
            ErrorMessage = "could not parse response JSON",
            Context = body
        });
        
        return Result<OpenIdTokenResponse, OpenIdError>.Success(parsed);
    }
}