using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class OidPresentationRecord : RecordBase
    {
        public string ClientId { get; set; } = null!;
        
        public string? ClientMetadata { get; set; }
        
        public PresentedCredential[] PresentedCredentials { get; set; } = null!;

        public override string TypeName => "AF.OidPresentationRecord";
    }
}
