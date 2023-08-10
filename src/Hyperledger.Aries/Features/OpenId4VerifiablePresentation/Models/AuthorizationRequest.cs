using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Web;
using Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Helpers;
using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models
{
    public class AuthorizationRequest
    {
        [JsonProperty(PropertyName = "response_type")]
        public string ResponseType { get; set; }
        
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }
        
        [JsonProperty(PropertyName = "redirect_uri")]
        public string RedirectUri { get; set; }
        
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        
        [JsonProperty(PropertyName = "presentation_definition")]
        public JObject PresentationDefinition { get; set; }
        
        [JsonProperty(PropertyName = "presentation_definition_uri")]
        public string PresentationDefinitionUri { get; set; }
        
        [JsonProperty(PropertyName = "nonce")]
        public string Nonce { get; set; }
        
        [JsonProperty(PropertyName = "registration")]
        public string Registration { get; set; }
        
        [JsonProperty(PropertyName = "response_mode")]
        public string ResponseMode { get; set; }

        public static AuthorizationRequest ParseFromUri(Uri uri)
        {
            if (uri.Scheme != "openid") return null;

            if (uri.ToString().Contains("%"))
                uri = new Uri(Uri.UnescapeDataString(uri.ToString()));

            var query = HttpUtility.ParseQueryString(uri.Query);

            if (query["request_uri"] != null)
            {
                // Todo: Resolve request uri
            }

            var newRequest = new AuthorizationRequest();
            foreach (var property in typeof(AuthorizationRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string queryParam = property.Name.ToSnakeCase();
                string value = query[queryParam];
                // if (value == null)
                //     throw new ArgumentNullException(queryParam,
                //         $"Authorization request is missing a parameter: {queryParam}");
                property.SetValue(newRequest, value);
            }

            return newRequest;
        }
        
        public static AuthorizationRequest ParseFromBase64Url(string uri)
        {
            return JsonConvert.DeserializeObject<AuthorizationRequest>(Base64UrlEncoder.Decode(uri));
        }

        public static AuthorizationRequest ParseFromJwt(string jwt)
        {
            // Todo: Return AuthorizationRequest
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadToken(jwt) as JwtSecurityToken;
            var json = token?.Payload.SerializeToJson() ?? "";
            var jObject = JObject.Parse(json);
            
            var newRequest = new AuthorizationRequest();
            foreach (var property in typeof(AuthorizationRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string propertyName = property.Name.ToSnakeCase();
                string value = jObject.GetValue(propertyName)?.ToString();
                // if (value == null)
                //     throw new ArgumentNullException(queryParam,
                //         $"Authorization request is missing a parameter: {queryParam}");
                property.SetValue(newRequest, value);
            }

            return newRequest;
        }
    }
}
