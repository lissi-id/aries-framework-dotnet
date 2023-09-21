using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Tests.Features.Pex.Models;
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
            var json = PexTestsDataProvider.GetPresentationDefinitionJson();

            var presentationDefinition = await _pexService.ParsePresentationDefinition(json);
            
            presentationDefinition.Id.Should().Be("123");
            presentationDefinition.Name.Should().Be("Scalable trust example");
            presentationDefinition.InputDescriptors.Length.Should().Be(1);
            presentationDefinition.Formats.Count.Should().Be(6);
        }
    }
}
