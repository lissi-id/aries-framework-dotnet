using System;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.OpenID4VC.Vp.JsonConverter
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CustomConverter : JsonConverter<AuthorizationRequest>
    {
        public override AuthorizationRequest ReadJson(JsonReader reader, Type objectType, AuthorizationRequest existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the JSON token.
            JToken token = JToken.Load(reader);

            // Create an instance of A.
            AuthorizationRequest result = new AuthorizationRequest();

            // Check if the token is a string or a JObject.
            if (token.Type == JTokenType.String)
            {
                // If it's a string, set PropertyA directly.
                result.PresentationDefinition = token.Value<PresentationDefinition>();
            }
            else if (token.Type == JTokenType.Object)
            {
                // If it's an object, deserialize it into PropertyA.
                //result.PresentationDefinition = token.ToObject<string>(serializer);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, AuthorizationRequest value, JsonSerializer serializer)
        {
            // Serialize A back to JSON.
            JToken token = JToken.FromObject(value.PresentationDefinition);
            token.WriteTo(writer);
        }
    }

}
