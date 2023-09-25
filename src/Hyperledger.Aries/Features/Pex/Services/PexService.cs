using System;
using System.Linq;
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

        /// <inheritdoc />
        public Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, CredentialDescriptor[] credentialDescriptors)
        {
            var inputDescriptorIds = presentationDefinition.InputDescriptors.Select(x => x.Id);
            if (!credentialDescriptors.Select(x => x.Id).All(inputDescriptorIds.Contains))
                throw new ArgumentException("Missing descriptors for given input descriptors in presentation definition.", nameof(credentialDescriptors));
            
            var presentationSubmission = new PresentationSubmission
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = presentationDefinition.Id,
                DescriptorMap = credentialDescriptors.Cast<Descriptor>().ToArray()
            };
            
            return Task.FromResult(presentationSubmission);
        }
    }
}
