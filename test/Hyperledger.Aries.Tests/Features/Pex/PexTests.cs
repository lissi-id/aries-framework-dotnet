using FluentAssertions;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Tests.Features.Pex.Models;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.Pex
{
    public class PexTests
    {
        [Fact]
        public void Can_Parse_Input_Descriptors()
        {
            var json = PexTestsDataProvider.GetInputDescriptorsJson();

            var inputDescriptors = JsonConvert.DeserializeObject<InputDescriptors>(json);
            
            inputDescriptors.Value.Length.Should().Be(2);
            inputDescriptors.Value[0].Name.Should().Be("EU Driver's License");
            inputDescriptors.Value[1].Name.Should().Be("US Passport");
        }
    }
}
