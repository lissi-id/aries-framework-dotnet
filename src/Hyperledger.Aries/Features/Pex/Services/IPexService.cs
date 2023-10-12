using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.Pex.Services
{
    /// <summary>
    /// Pex Service.
    /// </summary>
    public interface IPexService
    {
        /// <summary>
        /// Creates a presentation submission.
        /// </summary>
        /// <param name="presentationDefinition">The presentation definition.</param>
        /// <param name="descriptorMapInfo">Data used to build Descriptor Maps.</param>
        /// <returns></returns>
        Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, 
            (string inputDescriptorId, string pathToVerifiablePresentation, string format)[] descriptorMapInfo);
    }
}
