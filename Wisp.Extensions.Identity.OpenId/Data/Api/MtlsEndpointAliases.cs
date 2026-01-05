// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json.Serialization;

namespace Wisp.Extensions.Identity.OpenId.Data.Api;

public class MtlsEndpointAliases
{
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = "";

    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = "";

    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = "";

    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = "";

    [JsonPropertyName("registration_endpoint")]
    public string RegistrationEndpoint { get; set; } = "";

    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; set; } = "";

    [JsonPropertyName("pushed_authorization_request_endpoint")]
    public string PushedAuthorizationRequestEndpoint { get; set; } = "";

    [JsonPropertyName("backchannel_authentication_endpoint")]
    public string BackchannelAuthenticationEndpoint { get; set; } = "";
}