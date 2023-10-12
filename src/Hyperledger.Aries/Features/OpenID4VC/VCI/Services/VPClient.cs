using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Services
{
    public interface VPClient
    {
        public Task<AuthorizationRequest> ProcessAuthRequest(Uri authRequestUri);
        
    }
}
