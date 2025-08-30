using System.Text.Json.Serialization;

namespace Wisp.Extensions.Identity.OpenId.Data.Api;

public class OpenIdDiscovery
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = "";

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = "";

    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = "";

    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = "";

    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; set; } = "";

    [JsonPropertyName("end_session_endpoint")]
    public string EndSessionEndpoint { get; set; } = "";

    [JsonPropertyName("frontchannel_logout_session_supported")]
    public bool FrontchannelLogoutSessionSupported { get; set; }

    [JsonPropertyName("frontchannel_logout_supported")]
    public bool FrontchannelLogoutSupported { get; set; }

    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = "";

    [JsonPropertyName("check_session_iframe")]
    public string CheckSessionIframe { get; set; } = "";

    [JsonPropertyName("grant_types_supported")]
    public List<string> GrantTypesSupported { get; set; } = new();

    [JsonPropertyName("acr_values_supported")]
    public List<string> AcrValuesSupported { get; set; } = new();

    [JsonPropertyName("response_types_supported")]
    public List<string> ResponseTypesSupported { get; set; } = new();

    [JsonPropertyName("subject_types_supported")]
    public List<string> SubjectTypesSupported { get; set; } = new();

    [JsonPropertyName("prompt_values_supported")]
    public List<string> PromptValuesSupported { get; set; } = new();

    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public List<string> IdTokenSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("id_token_encryption_alg_values_supported")]
    public List<string> IdTokenEncryptionAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("id_token_encryption_enc_values_supported")]
    public List<string> IdTokenEncryptionEncValuesSupported { get; set; } = new();

    [JsonPropertyName("userinfo_signing_alg_values_supported")]
    public List<string> UserinfoSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("userinfo_encryption_alg_values_supported")]
    public List<string> UserinfoEncryptionAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("userinfo_encryption_enc_values_supported")]
    public List<string> UserinfoEncryptionEncValuesSupported { get; set; } = new();

    [JsonPropertyName("request_object_signing_alg_values_supported")]
    public List<string> RequestObjectSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("request_object_encryption_alg_values_supported")]
    public List<string> RequestObjectEncryptionAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("request_object_encryption_enc_values_supported")]
    public List<string> RequestObjectEncryptionEncValuesSupported { get; set; } = new();

    [JsonPropertyName("response_modes_supported")]
    public List<string> ResponseModesSupported { get; set; } = new();

    [JsonPropertyName("registration_endpoint")]
    public string RegistrationEndpoint { get; set; } = "";

    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public List<string> TokenEndpointAuthMethodsSupported { get; set; } = new();

    [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
    public List<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("introspection_endpoint_auth_methods_supported")]
    public List<string> IntrospectionEndpointAuthMethodsSupported { get; set; } = new();

    [JsonPropertyName("introspection_endpoint_auth_signing_alg_values_supported")]
    public List<string> IntrospectionEndpointAuthSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("authorization_signing_alg_values_supported")]
    public List<string> AuthorizationSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("authorization_encryption_alg_values_supported")]
    public List<string> AuthorizationEncryptionAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("authorization_encryption_enc_values_supported")]
    public List<string> AuthorizationEncryptionEncValuesSupported { get; set; } = new();

    [JsonPropertyName("claims_supported")]
    public List<string> ClaimsSupported { get; set; } = new();

    [JsonPropertyName("claim_types_supported")]
    public List<string> ClaimTypesSupported { get; set; } = new();

    [JsonPropertyName("claims_parameter_supported")]
    public bool ClaimsParameterSupported { get; set; }

    [JsonPropertyName("scopes_supported")]
    public List<string> ScopesSupported { get; set; } = new();

    [JsonPropertyName("request_parameter_supported")]
    public bool RequestParameterSupported { get; set; }

    [JsonPropertyName("request_uri_parameter_supported")]
    public bool RequestUriParameterSupported { get; set; }

    [JsonPropertyName("require_request_uri_registration")]
    public bool RequireRequestUriRegistration { get; set; }

    [JsonPropertyName("code_challenge_methods_supported")]
    public List<string> CodeChallengeMethodsSupported { get; set; } = new();

    [JsonPropertyName("tls_client_certificate_bound_access_tokens")]
    public bool TlsClientCertificateBoundAccessTokens { get; set; }

    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = "";

    [JsonPropertyName("revocation_endpoint_auth_methods_supported")]
    public List<string> RevocationEndpointAuthMethodsSupported { get; set; } = new();

    [JsonPropertyName("revocation_endpoint_auth_signing_alg_values_supported")]
    public List<string> RevocationEndpointAuthSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("backchannel_logout_supported")]
    public bool BackchannelLogoutSupported { get; set; }

    [JsonPropertyName("backchannel_logout_session_supported")]
    public bool BackchannelLogoutSessionSupported { get; set; }

    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = "";

    [JsonPropertyName("backchannel_token_delivery_modes_supported")]
    public List<string> BackchannelTokenDeliveryModesSupported { get; set; } = new();

    [JsonPropertyName("backchannel_authentication_endpoint")]
    public string BackchannelAuthenticationEndpoint { get; set; } = "";

    [JsonPropertyName("backchannel_authentication_request_signing_alg_values_supported")]
    public List<string> BackchannelAuthenticationRequestSigningAlgValuesSupported { get; set; } = new();

    [JsonPropertyName("require_pushed_authorization_requests")]
    public bool RequirePushedAuthorizationRequests { get; set; }

    [JsonPropertyName("pushed_authorization_request_endpoint")]
    public string PushedAuthorizationRequestEndpoint { get; set; } = "";

    [JsonPropertyName("mtls_endpoint_aliases")]
    public MtlsEndpointAliases MtlsEndpointAliases { get; set; } = new();

    [JsonPropertyName("authorization_response_iss_parameter_supported")]
    public bool AuthorizationResponseIssParameterSupported { get; set; }
}