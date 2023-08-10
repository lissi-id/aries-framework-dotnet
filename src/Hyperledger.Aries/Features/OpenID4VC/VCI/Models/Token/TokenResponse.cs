using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Token;

/// <summary>
/// 
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("c_nonce")]
    public string CNonce { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("c_nonce_expires_in")]
    public int CNonceExpiresIn { get; set; }
}
