using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest;

public class OidCredentialResponse
{
    /// <summary>
    ///     REQUIRED. JSON string denoting the format of the issued Credential.
    /// </summary>
    [JsonProperty("format")]
    public string Format { get; set; }

    /// <summary>
    ///     OPTIONAL. Contains issued Credential. MUST be present when acceptance_token is not returned. 
    ///     MAY be a JSON string or a JSON object, depending on the Credential format.
    /// </summary>
    [JsonProperty("credential")]
    public string Credential { get; set; }

    /// <summary>
    ///     OPTIONAL. A JSON string containing a security token subsequently used to obtain a Credential.
    ///     MUST be present when credential is not returned.
    /// </summary>
    [JsonProperty("acceptance_token")]
    public string AcceptanceToken { get; set; }

    /// <summary>
    ///     OPTIONAL. JSON string containing a nonce to be used to create a proof of possession of key material 
    ///     when requesting a Credential.
    /// </summary>
    [JsonProperty("c_nonce")]
    public string CNonce { get; set; }

    /// <summary>
    ///     OPTIONAL. JSON integer denoting the lifetime in seconds of the c_nonce.
    /// </summary>
    [JsonProperty("c_nonce_expires_in")]
    public int? CNonceExpiresIn { get; set; }
}
