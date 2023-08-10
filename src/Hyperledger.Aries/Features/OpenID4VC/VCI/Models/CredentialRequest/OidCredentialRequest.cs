using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest;

public class OidCredentialRequest
{
    public OidCredentialRequest(
        string format,
        string type,
        OidProofOfPossession proof)
    {
        Format = format;
        Type = type;
        Proof = proof;
    }

    [JsonProperty("format")]
    public string Format { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("proof")]
    public OidProofOfPossession Proof { get; set; }
}
