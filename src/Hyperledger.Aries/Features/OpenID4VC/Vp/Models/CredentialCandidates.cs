using System.Collections.Generic;
using Hyperledger.Aries.Storage.Models.Interfaces;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class CredentialCandidates
    {
        public bool LimitDisclosuresRequired { get; set; }
        
        public string InputDescriptorId { get; set; }
        
        public string[] Group { get; set; }
        
        public List<ICredential> Credentials { get; set; } = new List<ICredential>();
    }
}
