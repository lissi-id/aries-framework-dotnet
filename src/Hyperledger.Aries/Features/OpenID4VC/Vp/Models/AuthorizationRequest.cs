using Hyperledger.Aries.Features.Pex.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class AuthorizationRequest
    {
        [JsonProperty("response_type")]
        public string ResponseType { get; set; }
        
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        
        [JsonProperty("client_id_scheme")]
        public string ClientIdScheme { get; set; }
        
        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }
        
        [JsonProperty("scope")]
        public string Scope { get; set; }
        
        [JsonProperty("presentation_definition")]
        public JObject PresentationDefinitionAsString { get; set; }
        
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("response_mode")]
        public string ResponseMode { get; set; }
        
        [JsonProperty("state")]
        public string State { get; set; }
        
        [JsonIgnore]
        public PresentationDefinition PresentationDefinition { get; set; }

    }
}
