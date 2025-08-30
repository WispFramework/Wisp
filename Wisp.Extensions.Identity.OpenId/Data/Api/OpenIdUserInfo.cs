using System.Text.Json;
using System.Text.Json.Serialization;
using Wisp.Framework.SerDes;

namespace Wisp.Extensions.Identity.OpenId.Data.Api;

public class OpenIdUserInfo
{
    // Mandatory
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = null!;

    // Standard optional claims
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }

    [JsonPropertyName("middle_name")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }

    [JsonPropertyName("profile")]
    public string? Profile { get; set; }

    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool? EmailVerified { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("birthdate")]
    public string? Birthdate { get; set; }

    [JsonPropertyName("zoneinfo")]
    public string? ZoneInfo { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("phone_number_verified")]
    public bool? PhoneNumberVerified { get; set; }

    [JsonPropertyName("address")]
    public AddressObj? Address { get; set; }
    
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    
    [JsonPropertyName("roles")]
    public string? Roles { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extras { get; set; }
    
    [JsonPropertyName("string_extras")]
    public Dictionary<string, string>? StringExtras => Extras?
        .Where(kv => kv.Value.ValueKind == JsonValueKind.String)
        .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

    public class AddressObj
    {
        [JsonPropertyName("formatted")]
        public string? Formatted { get; set; }

        [JsonPropertyName("street_address")]
        public string? StreetAddress { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}