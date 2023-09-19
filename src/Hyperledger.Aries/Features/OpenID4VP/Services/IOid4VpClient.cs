using System.Threading.Tasks;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.OpenID4VP.Services
{
    public interface IOid4VpClient
    {
        Task<(CredentialCandidates[], string authorizationRequestRecordId)> ProcessAuthorizationRequest(string authorizationRequestUrl);
  
        Task SendAuthorizationResponse(SelectedCredential[] selectedCredentials, string authorizationRequestRecordId);
    }
}
