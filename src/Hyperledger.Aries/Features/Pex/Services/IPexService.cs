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
        /// Parses the presentation definition.
        /// </summary>
        /// <param name="presentationDefinitionJson">The JSON representation of a presentation definition.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the PresentationDefinition.</returns>
        Task<PresentationDefinition> ParsePresentationDefinition(string presentationDefinitionJson);
  
        /// <summary>
        /// Creates a presentation submission.
        /// </summary>
        /// <param name="presentationDefinition">The presentation definition.</param>
        /// <param name="credentialDescriptors">The credential descriptors.</param>
        /// <returns></returns>
        Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, CredentialDescriptor[] credentialDescriptors);
    }
}
