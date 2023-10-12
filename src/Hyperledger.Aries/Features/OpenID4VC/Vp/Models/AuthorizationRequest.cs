using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using Hyperledger.Aries.Features.Pex.Models;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class AuthorizationRequest
    {
        [JsonProperty("response_type")] 
        public string ResponseType { get; set; } = null!;

        [JsonProperty("client_id")] 
        public string ClientId { get; set; } = null!;

        [JsonProperty("client_id_scheme")] 
        public string? ClientIdScheme { get; set; }

        [JsonProperty("redirect_uri")] 
        public string? RedirectUri { get; set; }
        
        [JsonProperty("client_metadata")] 
        public string? ClientMetadata { get; set; }

        [JsonProperty("client_metadata_uri")] 
        public string? ClientMetadataUri { get; set; }

        [JsonProperty("scope")] 
        public string? Scope { get; set; }

        [JsonProperty("nonce")] 
        public string Nonce { get; set; } = null!;

        [JsonProperty("response_mode")] 
        public string? ResponseMode { get; set; }

        [JsonProperty("state")] 
        public string? State { get; set; }

        [JsonIgnore]
        public PresentationDefinition PresentationDefinition { get; set; } = null!;

        public static AuthorizationRequest ParseFromJwt(string jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadToken(jwt) as JwtSecurityToken;
            
            var authorizationRequest = JsonConvert.DeserializeObject<AuthorizationRequest>(token.Payload.SerializeToJson());
            authorizationRequest.PresentationDefinition = JsonConvert.DeserializeObject<PresentationDefinition>(token.Payload["presentation_definition"].ToString());
            
            return authorizationRequest;
        }

        public static AuthorizationRequest ParseFromUri(Uri uri)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);
            var dict = query.AllKeys.ToDictionary(key => key, key => query[key]);
            var json = JsonConvert.SerializeObject(dict);

            var authorizationRequest = JsonConvert.DeserializeObject<AuthorizationRequest>(json);
            
            var presentationDefinition = JsonConvert.DeserializeObject<PresentationDefinition>(query["presentation_definition"]);
            authorizationRequest.PresentationDefinition = presentationDefinition;

            return authorizationRequest;
        }
    }
}
