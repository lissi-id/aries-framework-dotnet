using System.Text.Json;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models.PresentationExchange
{
    public class PresentationDefinition
    {
        public string Id { get; set; }
        public InputDescriptor[] InputDescriptors { get; set; }
        public SubmissionRequirement[] SubmissionRequirements { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public Format Format { get; set; }

        public static PresentationDefinition FromJson(string json)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var options = new JsonSerializerSettings { ContractResolver = contractResolver};
            
            // var options = new JsonSerializerOptions
            // {
            //     PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            // };
            return JsonConvert.DeserializeObject<PresentationDefinition>(json, options);
        }

        // public string ToJson()
        // {
        //     var options = new JsonSerializerOptions
        //     {
        //         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //         PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        //     };
        //
        //     return JsonSerializer.Serialize(this, options);
        // }
    }
}
