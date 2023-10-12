using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Tests.Extensions;
using Hyperledger.Aries.Tests.Features.Pex.Models;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.Pex.Services
{
    public class PexServiceTests
    {
        private readonly PexService _pexService = new PexService();

        [Fact]
        public async Task Can_Create_Presentation_Submission()
        {
            var presentationDefinition = JsonConvert.DeserializeObject<PresentationDefinition>(PexTestsDataProvider.GetJsonForTestCase());
            
            var mappings = new (string inputDescriptorId, string pathToVerifiablePresentation, string format)[]
            {
                (presentationDefinition.InputDescriptors[0].Id, "$.credentials[0]", "vc+sd-jwt"),
                (presentationDefinition.InputDescriptors[1].Id, "$.credentials[1]", "vc+sd-jwt")
            };

            var presentationSubmission = await _pexService.CreatePresentationSubmission(presentationDefinition, mappings);

            presentationSubmission.Id.Should().NotBeNullOrWhiteSpace();
            presentationSubmission.DefinitionId.Should().Be(presentationDefinition.Id);
            presentationSubmission.DescriptorMap.Length.Should().Be(mappings.Length);

            for (var i = 0; i < presentationDefinition.InputDescriptors.Length; i++)
            {
                presentationSubmission.DescriptorMap[i].Id.Should().Be(presentationDefinition.InputDescriptors[i].Id);
                presentationSubmission.DescriptorMap[i].Format.Should().Be(mappings[i].format);
                presentationSubmission.DescriptorMap[i].Path.Should().Be(mappings[i].pathToVerifiablePresentation);   
            }
        }
        
        [Fact]
        public async Task Throws_Exception_When_Descriptors_Are_Missing()
        {
            var inputDescriptor = new InputDescriptor();
            inputDescriptor.PrivateSet(x => x.Id, Guid.NewGuid().ToString());
            inputDescriptor.PrivateSet(x => x.Formats, new Dictionary<string, Format> { {"format-1", null }});
            
            var presentationDefinition = new PresentationDefinition();
            presentationDefinition.PrivateSet(x => x.Id, Guid.NewGuid().ToString());
            presentationDefinition.PrivateSet(x => x.InputDescriptors, new[] { inputDescriptor });
            
            var mappings = new []
            {
                (Guid.NewGuid().ToString(), "$.credentials[0]", "vc+sd-jwt")
            };
        
            await Assert.ThrowsAsync<ArgumentException>(() => _pexService.CreatePresentationSubmission(presentationDefinition, mappings));
        }
    }
}
