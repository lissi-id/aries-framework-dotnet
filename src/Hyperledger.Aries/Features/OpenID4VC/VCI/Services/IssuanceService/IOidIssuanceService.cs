#nullable enable

using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialResponse;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Services.IssuanceService
{
    /// <summary>
    ///     Provides an interface for services related to OpenID for Verifiable Credential Issuance.
    /// </summary>
    public interface IOidIssuanceService
    {
        /// <summary>
        ///     Fetches the metadata related to the OID issuer from the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint URL to retrieve the issuer metadata.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OID issuer metadata.</returns>
        Task<OidIssuerMetadata> FetchIssuerMetadataAsync(string endpoint);

        /// <summary>
        ///     Requests a verifiable credential using the provided parameters.
        /// </summary>
        /// <param name="credentialIssuer">The credential issuer's identifier.</param>
        /// <param name="clientNonce">A nonce value for the client.</param>
        /// <param name="type">The type of the credential being requested.</param>
        /// <param name="tokenResponse">The token response from the previous token request.</param>
        /// <returns>
        ///     A tuple containing the credential response and the key alias used during the signing of the Proof of Possession
        ///     JWT.
        /// </returns>
        /// <remarks>
        ///     The returned key alias allows referencing a specific hardware key that was used for signing.
        /// </remarks>
        Task<(OidCredentialResponse, string)> RequestCredentialAsync(
            string credentialIssuer,
            string clientNonce,
            string type,
            TokenResponse tokenResponse);

        /// <summary>
        ///     Requests a token using the provided issuer metadata and pre-authorized code.
        /// </summary>
        /// <param name="metadata">The OID issuer metadata.</param>
        /// <param name="preAuthorizedCode">The pre-authorized code for token request.</param>
        /// <param name="pin">The PIN.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the token response.</returns>
        Task<TokenResponse> RequestTokenAsync(OidIssuerMetadata metadata, string preAuthorizedCode, string? pin = null);
    }
}
