using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenID4VP.Models;

namespace Hyperledger.Aries.Features.OpenID4VP.Services
{
    public interface IOid4VpClient
    {
        Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl);
  
        Task SendAuthorizationResponse(SelectedCredential[] selectedCredentials, string authorizationRequestRecordId);
    }
}
