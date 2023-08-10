using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest;

public class OidProofOfPossession
{
    public OidProofOfPossession(string proofType, string jwt)
    {
        ProofType = proofType;
        Jwt = jwt;
    }

    [JsonProperty("proof_type")]
    public string ProofType { get; set; }
    
    [JsonProperty("jwt")]
    public string Jwt { get; set; }
    
    
}
