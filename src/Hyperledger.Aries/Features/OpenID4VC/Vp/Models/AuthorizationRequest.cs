using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Web;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Pex.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        //[JsonProperty("presentation_definition")]
        public PresentationDefinition? PresentationDefinition { get; set; }


        public static AuthorizationRequest? ParseFromJwt(string jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadToken(jwt) as JwtSecurityToken;
            var json = JsonConvert.SerializeObject(token?.Payload.ToDictionary(key => key.Key,
                key => token.Payload[key.Key]));

            return JsonConvert.DeserializeObject<AuthorizationRequest>(json);
        }

        public static AuthorizationRequest? ParseFromUri(Uri uri)
        {
            if (uri.ToString().Contains("%"))
                uri = new Uri(Uri.UnescapeDataString(uri.ToString()));

            var query = HttpUtility.ParseQueryString(uri.Query);

            var dict = query.AllKeys.ToDictionary(key => key, key => query[key]);
            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            var obj = JObject.Parse(json)["presentation_definition"];
            var some = obj.ToObject<PresentationDefinition>();
            var x = JsonConvert.DeserializeObject<AuthorizationRequest>(json);
            x.PresentationDefinition = some;

            return x;
        }
    }
}
