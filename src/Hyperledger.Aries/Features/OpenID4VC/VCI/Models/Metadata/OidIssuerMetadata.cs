#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata
{
    /// <summary>
    ///     Represents the metadata of an OpenID4VCI Credential Issuer.
    /// </summary>
    public class OidIssuerMetadata
    {
        /// <summary>
        ///     Gets or sets a list of display properties of a Credential Issuer for different languages.
        /// </summary>
        [JsonProperty("display")]
        public List<Display>? Display { get; set; }

        /// <summary>
        ///     Gets or sets a list of metadata about separate credential types that the Credential Issuer can issue.
        /// </summary>
        [JsonProperty("credentials_supported")]
        public OidCredentialMetadata[] CredentialsSupported { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the URL of the Credential Issuer's Credential Endpoint.
        /// </summary>
        [JsonProperty("credential_endpoint")]
        public string CredentialEndpoint { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the identifier of the Credential Issuer.
        /// </summary>
        [JsonProperty("credential_issuer")]
        public string CredentialIssuer { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the identifier of the OAuth 2.0 Authorization Server that the Credential Issuer relies on for
        ///     authorization.
        ///     If this property is omitted, it is assumed that the entity providing the Credential Issuer
        ///     is also acting as the Authorization Server. In such cases, the Credential Issuer's
        ///     identifier is used as the OAuth 2.0 Issuer value to obtain the Authorization Server
        ///     metadata.
        /// </summary>
        [JsonProperty("authorization_server")]
        public string? AuthorizationServer { get; set; }
    }
}
