using System.Collections.Generic;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class PresentedCredential
    {
        public Dictionary<string, string> PresentedClaims { get; set; }
        
        public string CredentialId { get; set; }
    }
}
