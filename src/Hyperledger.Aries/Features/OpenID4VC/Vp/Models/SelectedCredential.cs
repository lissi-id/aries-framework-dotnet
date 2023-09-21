using Hyperledger.Aries.Storage.Models.Interfaces;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class SelectedCredential
    {
        public string InputDescriptorId { get; set; }
        
        public ICredential Credential { get; set; }
    }
}
