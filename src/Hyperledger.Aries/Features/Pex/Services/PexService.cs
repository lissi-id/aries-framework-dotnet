using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Pex.Services
{
    /// <inheritdoc />
    public class PexService : IPexService
    {
        /// <inheritdoc />
        public Task<PresentationDefinition> ParsePresentationDefinition(string presentationDefinition)
        {
            var parsed = JsonConvert.DeserializeObject<PresentationDefinition>(presentationDefinition);
            return Task.FromResult(parsed!);
        }
        
        public Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, CredentialDescriptor[] credentials)
        {
            throw new System.NotImplementedException();
        }
    }
}
