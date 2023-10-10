using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.Pex.Services
{
    public interface IPexService
    {
        Task<PresentationDefinition> ParsePresentationDefinition(string presentationDefinition);
  
        Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, DescriptorMap[] credentials);
    }
}
