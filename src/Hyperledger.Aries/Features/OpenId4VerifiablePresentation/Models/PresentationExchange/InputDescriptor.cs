using Newtonsoft.Json.Schema;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models.PresentationExchange
{
    public class InputDescriptor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string[] Group { get; set; }
        public Schema[] Schema { get; set; }
        public Constraints Constraints { get; set; }
        public Format Format { get; set; }
    }
    
    public class Schema
    {
        public object Uri { get; set; }
        public bool Required { get; set; }
    }

    public class Constraints
    {
        public string LimitDisclosure { get; set; }
        public Field[] Fields { get; set; }
    }

    public class Field
    {
        public string[] Path { get; set; }
        public string Id { get; set; }
        public string Purpose { get; set; }
        public string Name { get; set; }
        public Filter Filter { get; set; }
    }

    public class Filter
    {
        public string Type { get; set; }
        public string Pattern { get; set; }
    }
}
