using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.Pex.Services
{
    /// <inheritdoc />
    public class PexService : IPexService
    {
        /// <inheritdoc />
        public Task<PresentationSubmission> CreatePresentationSubmission(PresentationDefinition presentationDefinition, (string inputDescriptorId, string pathToVerifiablePresentation, string format)[] mapping)
        {
            var inputDescriptorIds = presentationDefinition.InputDescriptors.Select(x => x.Id);
            if (!mapping.All(descriptor => inputDescriptorIds.Contains(descriptor.inputDescriptorId)))
                throw new ArgumentException("Missing descriptors for given input descriptors in presentation definition.", nameof(mapping));
            
            var descriptorMaps = new List<DescriptorMap>();
            for (int index = 0; index < mapping.Count(); index++)
            {
                var descriptorMap = new DescriptorMap
                {
                    Format = mapping[index].format,
                    Path = mapping[index].pathToVerifiablePresentation,
                    Id = mapping[index].inputDescriptorId,
                    PathNested = null
                };
                descriptorMaps.Add(descriptorMap);
            }
            
            var presentationSubmission = new PresentationSubmission
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = presentationDefinition.Id,
                DescriptorMap = descriptorMaps.ToArray()
            };
            
            return Task.FromResult(presentationSubmission);
        }
    }
}
