namespace Hyperledger.Aries.Features.Pex.Models;

public class CredentialDescriptor
{
    public string InputDescriptorId { get; set; }
        
    public string CredentialId { get; set; }
        
    public string Format { get; set; }
        
    public string Path { get; set; }
        
    public CredentialDescriptor? PathNested { get; set; }
}
