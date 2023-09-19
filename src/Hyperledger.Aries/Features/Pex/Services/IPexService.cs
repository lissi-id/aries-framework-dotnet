using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.Pex.Services
{
    public interface IPexService
    {
        //TODO: Method may be extended with JSON Schema (tbd)
        Task<PresentationDefinition> ParsePresentationDefinition(string presentationDefinition);
  
        Task<PresentationSubmission> CreatePresentationSubmisson(string presentationDefinitionId, CredentialDescriptor[] credentials);
    }

    public class CredentialDescriptor
    {
        public string InputDescriptorId { get; set; }
        
        public string CredentialId { get; set; }
        
        public string Format { get; set; }
        
        public string Path { get; set; }
        
        public CredentialDescriptor? PathNested { get; set; }
    }
}
