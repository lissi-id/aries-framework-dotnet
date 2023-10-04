using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    public class AuthorizationRequestRecord : RecordBase
    {
        public override string TypeName { get; }
        
        public AuthorizationRequest AuthorizationRequest { get; set; }
    }
}
