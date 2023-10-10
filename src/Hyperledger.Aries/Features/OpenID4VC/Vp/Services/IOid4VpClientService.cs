using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public interface IOid4VpClientService
    {
        
        /// <summary>
        ///     Processes an OpenID4VP Authorization Request Url.
        /// </summary>
        /// <param name="authorizationRequestUrl"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the Authorization Response object associated with the OpenID4VP Authorization Request Url.
        /// </returns>
        public Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl);

        
        //Task<AuthorizationResponse> CreateAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials, PresentationSubmission presentationSubmission);
        
        /// <summary>
        ///     Creates the Parameters that are necessary to send an OpenId4VP Authorization Response.
        /// </summary>
        /// <param name="authorizationRequest"></param>
        /// /// <param name="selectedCredentials"></param>
        /// <param name="presentationSubmission"></param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the Presentation Submission and the VP Token.
        /// </returns>
        Task<AuthorizationResponse> CreateAuthorizationResponse(string[] vpToken, PresentationSubmission presentationSubmission);
    }
}
