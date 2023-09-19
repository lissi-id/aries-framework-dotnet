namespace Hyperledger.Aries.Features.Pex.Models
{
    public class SelectedCredential
    {
        public ICredential CredentialId { get; set; }
        
        public string InputDescriptorId { get; set; }
    }

    public interface ICredential
    {
        public string CredentialId { get; set; }
    }
}
