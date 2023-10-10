namespace Hyperledger.Aries.Features.Pex.Models
{
    public class DescriptorMap
    {
        public string InputDescriptorId { get; set; }
        
        public string Format { get; set; }
        
        public string Path { get; set; }
        
        public DescriptorMap? PathNested { get; set; }
    }
}
