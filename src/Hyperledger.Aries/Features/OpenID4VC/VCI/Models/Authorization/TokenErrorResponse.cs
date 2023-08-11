using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;

/// <summary>
///     Represents an error response from the OAuth 2.0 Authorization Server when the token request is invalid or
///     unauthorized.
/// </summary>
public class TokenErrorResponse
{
    /// <summary>
    ///     Gets or sets the error code indicating the type of error that occurred.
    /// </summary>
    [JsonProperty("error")]
    public string Error { get; set; }

    /// <summary>
    ///     Gets or sets the human-readable text providing additional information about the error.
    /// </summary>
    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; }

    /// <summary>
    ///     Gets or sets the URI identifying a human-readable web page with information about the error.
    /// </summary>
    [JsonProperty("error_uri")]
    public string ErrorUri { get; set; }
}
