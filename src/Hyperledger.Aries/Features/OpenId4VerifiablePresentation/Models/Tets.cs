using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models
{
    public class Tets
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }
    }
}
