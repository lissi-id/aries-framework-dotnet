using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Token;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Services;

/// <summary>
/// 
/// </summary>
public interface IOidCredentialService
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task GetAsync();
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task ListAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<OidIssuerMetadata> FetchIssuerMetadataAsync(string endpoint);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<TokenResponse> RequestTokenAsync(OidIssuerMetadata metadata, string preAuthorizedCode);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task RequestCredentialAsync(string credentialIssuer, string clientNonce, string type, TokenResponse tokenResponse);
}
