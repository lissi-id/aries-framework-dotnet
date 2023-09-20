using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Pex.Models
{
    public class InputDescriptor
    {
        [JsonProperty("constraints")] 
        public Constraint[] Constraints { get; set; }

        [JsonProperty("format")]
        public Format? Format { get; set; }

        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("group")]
        public string? Group { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("purpose")]
        public string? Purpose { get; set; }
    }

    public class Format
    {
        public Dictionary<string, Algorithm> SupportedAlgorithms { get; set; }
    }

    public class Algorithm
    {
        [JsonProperty("alg")] 
        public string[] Alg { get; set; }
    }

    public class Constraint
    {
        [JsonProperty("fields")]
        public Field[] Fields { get; set; }

        // MUST be required or preferred
        [JsonProperty("limit_disclosure")] 
        public string LimitDisclosure { get; set; }
    }

    public class Field
    {
        [JsonProperty("filter")]
        public Filter Filter { get; set; }

        // only one entry
        [JsonProperty("path")]
        public string[] Path { get; set; }
    }

    public class Filter
    {
        // must be string
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("const")]
        public string Const { get; set; }
    }
}
