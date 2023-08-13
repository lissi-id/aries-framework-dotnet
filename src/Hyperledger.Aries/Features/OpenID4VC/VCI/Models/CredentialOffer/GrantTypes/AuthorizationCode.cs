#nullable enable

using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialOffer.GrantTypes
{
    /// <summary>
    ///     Represents the parameters for the 'authorization_code' grant type.
    /// </summary>
    public class AuthorizationCode
    {
        /// <summary>
        ///     Gets or sets an optional string value created by the Credential Issuer, opaque to the Wallet, that is used to bind
        ///     the subsequent Authorization Request with the Credential Issuer to a context set up during previous steps.
        /// </summary>
        [JsonProperty("issuer-state")]
        public string? IssuerState { get; set; }
    }
}
