using System.Collections.Generic;
using Hyperledger.Aries.Storage.Models.Interfaces;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class CredentialCandidates
    {
        public bool LimitDisclosuresRequired { get; set; }
        
        public string InputDescriptorId { get; set; }
        
<<<<<<< HEAD
        public string[] Group { get; set; }
        
=======
>>>>>>> d05d264 (implement GetCredentialCandidates with fields and filters in SdJwtVcHolderService)
        public List<ICredential> Credentials { get; set; } = new List<ICredential>();
    }
}
