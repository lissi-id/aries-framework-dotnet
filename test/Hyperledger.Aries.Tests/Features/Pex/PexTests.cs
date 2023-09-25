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
            var json = PexTestsDataProvider.GetJsonForTestCase();

            var inputDescriptors = JsonConvert.DeserializeObject<InputDescriptor[]>(json);
            
            inputDescriptors.Length.Should().Be(2);
            inputDescriptors[0].Name.Should().Be("EU Driver's License");
            inputDescriptors[1].Name.Should().Be("US Passport");
        }
    }
}
