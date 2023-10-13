using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Services;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Features.OpenID4VC.Vp.Controller
{
    public class Oid4VpController : IOid4VpController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAgentProvider _agentProvider;
        private readonly ISdJwtVcHolderService _sdJwtVcHolderService;
        private readonly IPexService _pexService;
        private readonly IOid4VpClientService _oid4VpClientService;

        public Oid4VpController(
            IHttpClientFactory httpClientFactory,
            IAgentProvider agentProvider,
            ISdJwtVcHolderService sdJwtVcHolderService,
            IPexService pexService,
            IOid4VpClientService oid4VpClientService)
        {
            _httpClientFactory = httpClientFactory;
            _agentProvider = agentProvider;
            _sdJwtVcHolderService = sdJwtVcHolderService;
            _pexService = pexService;
            _oid4VpClientService = oid4VpClientService;
        }

        /// <inheritdoc />
        public async Task<(AuthorizationRequest, CredentialCandidates[])> ProcessAuthorizationRequest(string authorizationRequestUrl)
        {
            var uri = new Uri(authorizationRequestUrl);
            
            if (uri.Scheme != "haip") 
                throw new InvalidOperationException("HAIP Profile requires haip scheme");
            
            if (String.IsNullOrEmpty(uri.GetQueryParam("request_uri")))
                throw new InvalidOperationException("HAIP Profile requires request_uri parameter");
            
            var authorizationRequest = await _oid4VpClientService.ProcessAuthorizationRequest(authorizationRequestUrl);

            if (IsAuthorizationRequestHaipConform(authorizationRequest))
                throw new InvalidOperationException("Authorization Request is not HAIP conform");
            
            var agentContext = await _agentProvider.GetContextAsync();
            var credentials = await _sdJwtVcHolderService.ListAsync(agentContext);
            return (authorizationRequest, await _sdJwtVcHolderService.FindCredentialCandidates(credentials.ToArray(),
                authorizationRequest.PresentationDefinition.InputDescriptors));
        }


        /// <inheritdoc />
        public async Task<AuthorizationResponse> PrepareAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials)
        {
            var descriptorMaps = new List<(string, string, string)>();
            var presentationFormats = new List<string>();
            for (var index = 0; index < selectedCredentials.Length; index++)
            {
                //TODO: Other Credential Types than SD-JWT
                
                var inputDescriptor = authorizationRequest.PresentationDefinition.InputDescriptors
                    .FirstOrDefault(x => x.Id == selectedCredentials[index].InputDescriptorId);

                var presentationFormat = await _sdJwtVcHolderService.CreatePresentation((SdJwtRecord) selectedCredentials[index].Credential, GetDisclosureNamesFromInputDescriptor(inputDescriptor), authorizationRequest.ClientId, authorizationRequest.Nonce);
                presentationFormats.Add(presentationFormat);
                
                descriptorMaps.Add((inputDescriptor.Id, "$[" + index + "]", "vc+sd-jwt"));
            }
            
            var presentationSubmission = await _pexService.CreatePresentationSubmission(authorizationRequest.PresentationDefinition, descriptorMaps.ToArray());
            var authorizationResponse = _oid4VpClientService.CreateAuthorizationResponse(presentationFormats.ToArray(), presentationSubmission);
            
            return authorizationResponse;
        }


        /// <inheritdoc />
        public async Task SendAuthorizationResponse(Uri redirectUri, AuthorizationResponse authorizationResponse)
        {
            var content = new List<KeyValuePair<string, string>>();
            content.Add(new KeyValuePair<string, string>("vp_token", authorizationResponse.VpToken));
            content.Add(new KeyValuePair<string, string>("presentation_submission", authorizationResponse.PresentationSubmission));
            
            var request = new HttpRequestMessage
            {
                RequestUri = redirectUri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(content)
            };

            var httpClient = _httpClientFactory.CreateClient();
            await httpClient.SendAsync(request);
        }
        
        private static string[] GetDisclosureNamesFromInputDescriptor(InputDescriptor inputDescriptor)
        {
            var disclosureNames = new List<string>();

            foreach (var field in inputDescriptor.Constraints.Fields)
            {
                foreach (var path in field.Path)
                {
                    disclosureNames.Add(path.Split(".").Last());
                }
            }

            return disclosureNames.ToArray();
        }

        private bool IsAuthorizationRequestHaipConform(AuthorizationRequest authorizationRequest)
        {
            //TODO: Complete the validation
            
            if (authorizationRequest.ResponseType != "vp_token")
                return false;
                
            if (authorizationRequest.ClientId != "vp_token") 
                return false;
            
            if (authorizationRequest.ResponseMode != "direct_post") //redirect_rui?
                return false;
            
            if (!(authorizationRequest.ClientIdScheme == "x509_san_dns" || authorizationRequest.ClientIdScheme == "verifier_attestation")) 
                return false;

            return true;
        }
    }
}
