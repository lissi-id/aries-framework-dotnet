#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialOffer;

/// <summary>
///     Represents an OpenID4VCI Credential Offer, which is used to obtain one or more credentials from a Credential Issuer.
/// </summary>
public class OidCredentialOffer
{
    /// <summary>
    ///     Creates an instance of the OidCredentialOffer class.
    /// </summary>
    /// <param name="credentialIssuer">
    ///     The URL of the Credential Issuer from which the Wallet is requested to obtain one or
    ///     more Credentials.
    /// </param>
    /// <param name="credentials">
    ///     A list of credentials that the Wallet may request. Each credential in the list must contain a
    ///     format Claim determining the format of the credential to be requested and further parameters characterising the
    ///     type of the credential to be requested.
    /// </param>
    public OidCredentialOffer(
        string credentialIssuer,
        List<OidCredential> credentials)
    {
        CredentialIssuer = credentialIssuer;
        Credentials = credentials;
    }
    
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
    public List<OidCredential> Credentials { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the Credential Issuer from where the Wallet is requested to obtain one or more Credentials from.
    /// </summary>
    [JsonProperty("credential_issuer")]
    public string CredentialIssuer { get; set; }
}
