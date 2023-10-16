using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class OidPresentationRecord : RecordBase
    {
        public string? ClientId { get; set; }
        
        public string? ClientMetadata { get; set; }
        
        public PresentedCredential[] PresentedCredentials { get; set; }

        public override string TypeName => "AF.OidPresentationRecord";
    }
}
