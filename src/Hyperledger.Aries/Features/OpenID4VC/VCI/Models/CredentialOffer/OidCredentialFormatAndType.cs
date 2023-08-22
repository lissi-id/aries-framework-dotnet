using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialOffer
{
    /// <summary>
    ///     Represents a credential that the Wallet may request from the Credential Issuer as part of the OpenID4VCI Credential
    ///     Offer.
    /// </summary>
    public class OidCredentialFormatAndType
    {
        /// <summary>
        ///     Gets or sets the format claim of the credential. This determines the format of the credential to be requested.
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }

        /// <summary>
        ///     Gets or sets the type of the credential to be requested.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
