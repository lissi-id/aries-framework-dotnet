using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;

namespace Hyperledger.Aries.Features.OpenID4VC.Vp.Controller
{
    /// <summary>
    ///   This Service offers methods to handle the OpenId4Vp protocol according to the HAIP
    /// </summary>
    public interface IOid4VpController
    {
        /// <summary>
        ///     Processes an OpenID4VP Authorization Request Url.
        /// </summary>
        /// <param name="authorizationRequestUrl"></param>
        /// /// <param name="agentContext"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the Authorization Response object associated with the OpenID4VP Authorization Request Url.
        /// </returns>
        public Task<(AuthorizationRequest authorizationRequest, CredentialCandidates[] credentialCandidates)> ProcessAuthorizationRequest(Uri authorizationRequestUrl, IAgentContext agentContext);

        /// <summary>
        ///     Prepares the Authorization Response.
        /// </summary>
        /// <param name="authorizationRequest"></param>
        /// <param name="selectedCredentials"></param>
        /// /// <param name="agentContext"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the AuthorizationResponse including the Presentation Submission and the VP Token.
        /// </returns>
        public Task<AuthorizationResponse> PrepareAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials, IAgentContext agentContext);
        
        /// <summary>
        ///     Sends an Authorization Response containing a Presentation Submission and the VP Token to the Redirect Uri.
        /// </summary>
        /// <param name="responseUri"></param>
        /// <param name="authorizationResponse"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the Callback Url of the Authorization Response if present.
        /// </returns>
        public Task<string?> SendAuthorizationResponse(Uri responseUri, AuthorizationResponse authorizationResponse);
    }
}
