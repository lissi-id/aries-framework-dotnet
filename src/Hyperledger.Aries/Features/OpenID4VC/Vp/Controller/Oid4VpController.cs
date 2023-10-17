using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Services;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.OpenID4VC.Vp.Controller
{
    /// <inheritdoc />
    public class Oid4VpController : IOid4VpController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISdJwtVcHolderService _sdJwtVcHolderService;
        private readonly IOid4VpClientService _oid4VpClientService;
        private readonly IWalletRecordService _walletRecordService;

        public Oid4VpController(
            IHttpClientFactory httpClientFactory,
            ISdJwtVcHolderService sdJwtVcHolderService,
            IOid4VpClientService oid4VpClientService,
            IWalletRecordService walletRecordService)
        {
            _httpClientFactory = httpClientFactory;
            _sdJwtVcHolderService = sdJwtVcHolderService;
            _oid4VpClientService = oid4VpClientService;
            _walletRecordService = walletRecordService;
        }

        /// <inheritdoc />
        public async Task<(AuthorizationRequest, CredentialCandidates[])> ProcessAuthorizationRequest(Uri authorizationRequestUri, IAgentContext agentContext)
        {
            if (authorizationRequestUri.Scheme != "haip") 
                throw new InvalidOperationException("HAIP requires haip scheme");
            
            if (String.IsNullOrEmpty(authorizationRequestUri.GetQueryParam("request_uri")))
                throw new InvalidOperationException("HAIP requires request_uri parameter");

            var authorizationRequest = await _oid4VpClientService.ProcessAuthorizationRequest(authorizationRequestUri);
            
            if (IsAuthorizationRequestHaipConform(authorizationRequest))
                throw new InvalidOperationException("Authorization Request is not HAIP conform");
            
            var credentials = await _sdJwtVcHolderService.ListAsync(agentContext);
            var credentialCandidates = await _sdJwtVcHolderService.FindCredentialCandidates(credentials.ToArray(),
                authorizationRequest.PresentationDefinition.InputDescriptors);

            return (authorizationRequest, credentialCandidates);
        }


        /// <inheritdoc/>
        public async Task<AuthorizationResponse> PrepareAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials, IAgentContext agentContext)
        {
            var presentationFormatMaps = new List<(string, string)>();
            var presentedCredentials = new List<PresentedCredential>();
            for (int index = 0; index < selectedCredentials.Length; index++)
            {
                var currentCredential = (SdJwtRecord)selectedCredentials[index].Credential;
                    
                var inputDescriptor = authorizationRequest.PresentationDefinition.InputDescriptors
                    .FirstOrDefault(x => x.Id == selectedCredentials[index].InputDescriptorId);
                if (inputDescriptor == null)
                    throw new InvalidOperationException($"Selected credential with ID: {currentCredential.Id} cannot be used to answer any Input Descriptors");

                var claimsToDisclose = GetDisclosureNamesFromInputDescriptor(inputDescriptor);
                var presentation = await _sdJwtVcHolderService.CreatePresentation(currentCredential, claimsToDisclose, authorizationRequest.ClientId, authorizationRequest.Nonce);
                
                presentationFormatMaps.Add((selectedCredentials[index].InputDescriptorId, presentation));
                
                presentedCredentials.Add(new PresentedCredential
                {
                    CredentialId = currentCredential.Id,
                    PresentedClaims = GetPresentedClaimsForCredential(inputDescriptor, currentCredential)
                });
            }

            var authorizationResponse = await _oid4VpClientService.CreateAuthorizationResponse(authorizationRequest, presentationFormatMaps.ToArray());
            
            await _walletRecordService.AddAsync(agentContext.Wallet, new OidPresentationRecord
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = authorizationRequest.ClientId,
                ClientMetadata = authorizationRequest.ClientMetadata,
                PresentedCredentials = presentedCredentials.ToArray()
            });
            
            return authorizationResponse;
        }


        /// <inheritdoc />
        public async Task<string?> SendAuthorizationResponse(Uri responseUri, AuthorizationResponse authorizationResponse)
        {
            var requestContent = new List<KeyValuePair<string, string>>();
            requestContent.Add(new KeyValuePair<string, string>("vp_token", authorizationResponse.VpToken));
            requestContent.Add(new KeyValuePair<string, string>("presentation_submission", authorizationResponse.PresentationSubmission));
            
            var request = new HttpRequestMessage
            {
                RequestUri = responseUri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(requestContent)
            };

            var httpClient = _httpClientFactory.CreateClient();
            var responseMessage = await httpClient.SendAsync(request);
            
            if (!responseMessage.IsSuccessStatusCode)
                throw new InvalidOperationException("Authorization Response could not be sent");

            if (responseMessage.Content == null)
                return null;

            var content = await responseMessage.Content.ReadAsStringAsync();
            return JObject.Parse(content)["redirect_uri"]?.ToString();
        }
        
        private static string[] GetDisclosureNamesFromInputDescriptor(InputDescriptor inputDescriptor)
        {
            var disclosureNames = new List<string>();
            
            if (inputDescriptor.Constraints.Fields == null)
                return null;
            
            foreach (var field in inputDescriptor.Constraints.Fields)
            {
                foreach (var path in field.Path)
                {
                    disclosureNames.Add(path.Split(".").Last());
                }
            }

            return disclosureNames.ToArray();
        }

        private static Dictionary<string, string> GetPresentedClaimsForCredential(InputDescriptor inputDescriptor, SdJwtRecord sdJwtRecord)
        {
            var presentedClaims = new Dictionary<string, string>();
            
            foreach (var field in inputDescriptor.Constraints.Fields)
            {
                foreach (var path in field.Path)
                {
                    var claim = sdJwtRecord.Claims.FirstOrDefault(x => x.Key == path.Split(".").Last());
                    presentedClaims.Add(claim.Key, claim.Value);
                }
            }

            return presentedClaims;
        }

        private bool IsAuthorizationRequestHaipConform(AuthorizationRequest authorizationRequest)
        {
            if (authorizationRequest.ResponseType != "vp_token")
                return false;
            
            if (authorizationRequest.ResponseMode != "direct_post")
                return false;
            
            if (authorizationRequest.ResponseMode == "direct_post" 
                && !String.IsNullOrEmpty(authorizationRequest.RedirectUri))
                return false; //TODO: throw invalid_request
            
            if (!String.IsNullOrEmpty(authorizationRequest.ResponseUri) &&
                !String.IsNullOrEmpty(authorizationRequest.RedirectUri))
                return false;
            
            if (authorizationRequest.ClientIdScheme == "redirect_uri"
                && authorizationRequest.ResponseMode == "direct_post"
                && !String.IsNullOrEmpty(authorizationRequest.RedirectUri)
                && authorizationRequest.ClientId != authorizationRequest.ResponseUri)
                return false;
            
            //TODO: Not supported yet
            //if (!(authorizationRequest.ClientIdScheme == "x509_san_dns" || authorizationRequest.ClientIdScheme == "verifier_attestation")) 
                //return false;

            return true;
        }
    }
}
