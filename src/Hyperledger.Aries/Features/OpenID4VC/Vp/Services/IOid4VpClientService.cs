using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public interface IOid4VpClientService
    {
        Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl);

        Task<(PresentationSubmission, string)> CreateAuthorizationResponse();
            
        Task SendAuthorizationResponse(SelectedCredential[] selectedCredentials, string authorizationRequestRecordId);
    }
}
