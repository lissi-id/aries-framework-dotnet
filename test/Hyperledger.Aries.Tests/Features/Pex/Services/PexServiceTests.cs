using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Tests.Extensions;
using Hyperledger.Aries.Tests.Features.Pex.Models;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.Pex.Services
{
    public class PexServiceTests
    {
        private readonly PexService _pexService = new();

        [Fact]
        public async Task Can_Parse_Presentation_Definition()
        {
            var json = PexTestsDataProvider.GetJsonForTestCase();

            var presentationDefinition = await _pexService.ParsePresentationDefinition(json);
            
            presentationDefinition.Id.Should().Be("123");
            presentationDefinition.Name.Should().Be("Parse example");
            presentationDefinition.InputDescriptors.Length.Should().Be(2);
            presentationDefinition.InputDescriptors.Length.Should().Be(2);
            presentationDefinition.InputDescriptors[0].Name.Should().Be("EU Driver's License");
            presentationDefinition.InputDescriptors[1].Name.Should().Be("US Passport");
            
            presentationDefinition.Formats.Count.Should().Be(6);
        }

        [Fact]
        public async Task Can_Create_Presentation_Submission()
        {
            var presentationDefinition = JsonConvert.DeserializeObject<PresentationDefinition>(PexTestsDataProvider.GetJsonForTestCase());
            
            var credentials = new CredentialDescriptor[]
            {
                new()
                {
                    Id = presentationDefinition.InputDescriptors[0].Id,
                    CredentialId = Guid.NewGuid().ToString(),
                    Format = presentationDefinition.InputDescriptors[0].Formats.First().Key,
                    Path = "$.credentials[0]"
                },
                new()
                {
                    Id = presentationDefinition.InputDescriptors[1].Id,
                    CredentialId = Guid.NewGuid().ToString(),
                    Format = presentationDefinition.InputDescriptors[1].Formats.First().Key,
                    Path = "$.credentials[1]"
                },
            };

            
            var presentationSubmission = await _pexService.CreatePresentationSubmission(presentationDefinition, credentials);

            presentationSubmission.Id.Should().NotBeNullOrWhiteSpace();
            presentationSubmission.DefinitionId.Should().Be(presentationDefinition.Id);
            presentationSubmission.DescriptorMap.Length.Should().Be(credentials.Length);

            for (var i = 0; i < presentationDefinition.InputDescriptors.Length; i++)
            {
                presentationSubmission.DescriptorMap[i].Id.Should().Be(presentationDefinition.InputDescriptors[i].Id);
                presentationSubmission.DescriptorMap[i].Format.Should().Be(credentials[i].Format);
                presentationSubmission.DescriptorMap[i].Path.Should().Be(credentials[i].Path);   
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
            
            var credentials = new CredentialDescriptor[]
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    CredentialId = Guid.NewGuid().ToString(),
                    Format = presentationDefinition.InputDescriptors[0].Formats.First().Key,
                    Path = "$.credentials[0]"
                }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _pexService.CreatePresentationSubmission(presentationDefinition, credentials));
        }
    }
}
