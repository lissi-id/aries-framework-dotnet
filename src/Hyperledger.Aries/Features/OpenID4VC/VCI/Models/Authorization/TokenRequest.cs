#nullable enable

using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;

/// <summary>
///     Represents a request for an access token from an OAuth 2.0 Authorization Server.
/// </summary>
public class TokenRequest
{
    /// <summary>
    ///     Gets or sets the grant type of the request. Determines the type of token request being made.
    /// </summary>
    [JsonProperty("grant_type")]
    public string GrantType { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the pre-authorized code. Represents the authorization to obtain specific credentials.
    ///     This is required if the grant type is urn:ietf:params:oauth:grant-type:pre-authorized_code.
    /// </summary>
    [JsonProperty("pre-authorized_code")]
    public string PreAuthorizedCode { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the scope of the access request. Defines the permissions the client is asking for.
    /// </summary>
    [JsonProperty("scope")]
    public string? Scope { get; set; }

    /// <summary>
    ///     Gets or sets the user PIN. This value must be present if a PIN was required in a previous step.
    /// </summary>
    [JsonProperty("user_pin")]
    public string? UserPin { get; set; }
}
