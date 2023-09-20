using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenID4VP.Models
{
    public class SelectedCredential
    {
        public string InputDescriptorId { get; set; }
        
        public ICredential Credential { get; set; }
    }
}
