#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.CredentialOffer
{
    /// <summary>
    ///     Represents an OpenID4VCI Credential Offer, which is used to obtain one or more credentials from a Credential
    ///     Issuer.
    /// </summary>
    public class OidCredentialOffer
    {
        /// <summary>
        ///     Gets or sets the JSON object indicating to the Wallet the Grant Types the Credential Issuer's AS is prepared to
        ///     process for this credential offer. If not present or empty, the Wallet must determine the Grant Types the
        ///     Credential Issuer's AS supports using the respective metadata.
        /// </summary>
        [JsonProperty("grants")]
        public Grants? Grants { get; set; }

        /// <summary>
        ///     Gets or sets the list of credentials that the Wallet may request. Each credential in the list must contain a format
        ///     Claim determining the format of the credential to be requested and further parameters characterising the type of
        ///     the credential to be requested.
        /// </summary>
        [JsonProperty("credentials")]
        public List<OidCredentialFormatAndType> Credentials { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the URL of the Credential Issuer from where the Wallet is requested to obtain one or more Credentials
        ///     from.
        /// </summary>
        [JsonProperty("credential_issuer")]
        public string CredentialIssuer { get; set; } = null!;
    }
}
