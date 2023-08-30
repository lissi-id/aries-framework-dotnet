using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest
{
    /// <summary>
    ///     Represents a credential request made by a client to the Credential Endpoint.
    ///     This request contains the format of the credential, the type of credential,
    ///     and a proof of possession of the key material the issued credential shall be bound to.
    /// </summary>
    public class OidCredentialRequest
    {
        /// <summary>
        ///     Gets or sets the proof of possession of the key material the issued credential shall be bound to.
        /// </summary>
        [JsonProperty("proof")]
        public OidProofOfPossession Proof { get; set; }

        /// <summary>
        ///     Gets or sets the format of the credential to be issued.
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }

        /// <summary>
        ///     Gets or sets the type of the credential.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
