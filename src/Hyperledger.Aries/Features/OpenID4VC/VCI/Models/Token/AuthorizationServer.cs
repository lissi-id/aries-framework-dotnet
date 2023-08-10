using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Token;

public class AuthorizationServer
{
    [JsonProperty("issuer")]
    public string Issuer { get; set; }
    
    [JsonProperty("token_endpoint")]
    public string TokenEndpoint { get; set; }
    
    [JsonProperty("token_endpoint_auth_methods_supported")]
    public string[] TokenEndpointAuthMethodsSupported { get; set; }
    
    [JsonProperty("token_endpoint_auth_signing_alg_values_supported")]
    public string[] TokenEndpointAuthSigningAlgValuesSupported { get; set; }
    
    [JsonProperty("response_types_supported")]
    public string[] ResponseTypesSupported { get; set; }
}
