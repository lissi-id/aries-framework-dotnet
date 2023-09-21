using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public interface IOid4VpClient
    {
        Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl);
  
        Task SendAuthorizationResponse(SelectedCredential[] selectedCredentials, string authorizationRequestRecordId);
    }
}
