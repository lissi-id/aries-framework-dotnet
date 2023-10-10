using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;

namespace Hyperledger.Aries.Features.OpenID4VC.Vp.Controller
{
    public interface IOid4VpController
    {
        /// <summary>
        ///     Processes an OpenID4VP Authorization Request Url.
        /// </summary>
        /// <param name="authorizationRequestUrl"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the Authorization Response object associated with the OpenID4VP Authorization Request Url.
        /// </returns>
        public Task<(AuthorizationRequest authorizationRequest, CredentialCandidates[] credentialCandidates)> ProcessAuthorizationRequest(string authorizationRequestUrl);

        public Task<AuthorizationResponse> PrepareAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials);
        
        /// <summary>
        ///     Sends an Authorization Response containing a Presentation Submission and the VP Token to the Redirect Uri.
        /// </summary>
        /// <param name="redirectUri"></param>
        /// <param name="authorizationResponse"></param>
        /// <returns>
        ///     A task representing the asynchronous operation.
        /// </returns>
        public Task SendAuthorizationResponse(Uri redirectUri, AuthorizationResponse authorizationResponse);
    }
}
