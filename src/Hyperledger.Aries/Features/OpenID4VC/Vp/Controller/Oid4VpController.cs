using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json.Linq;

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
            var json = "%7B%22id%22:%22vp%20token%20example%22,%22input_descriptors%22:%5B%7B%22id%22:%22id%20card%20credential%22,%22format%22:%7B%22ldp_vc%22:%7B%22proof_type%22:%5B%22Ed25519Signature2018%22%5D%7D%7D,%22constraints%22:%7B%22fields%22:%5B%7B%22path%22:%5B%22$.type%22%5D,%22filter%22:%7B%22type%22:%22string%22,%22pattern%22:%22IDCardCredential%22%7D%7D%5D%7D%7D%5D%7D";
            authorizationRequestUrl = "openid4vp://?response_type=vp_token&client_id=https%3A%2F%2Fclient.example.org%2Fcb&redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb&presentation_definition=" + json;
            
            var authorizationRequest = await _oid4VpClientService.ProcessAuthorizationRequest(authorizationRequestUrl);
            
            var agentContext = await _agentProvider.GetContextAsync();
            var credentials = await _sdJwtVcHolderService.ListAsync(agentContext);
            return (authorizationRequest, await _sdJwtVcHolderService.GetCredentialCandidates(credentials.ToArray(),
                authorizationRequest.PresentationDefinition.InputDescriptors));
        }


        public async Task<AuthorizationResponse> PrepareAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials)
        {
            
            // foreach selected
            //     CreatePresenation(credential)
            //     vpToken.Add(presentation);
            


            var descriptorMaps = new List<DescriptorMap>();
            var vpToken = new List<string>();
            for (var credential = 0; credential < selectedCredentials.Length; credential++)
            {
                //TODO: Other Credential Types than SD-JWT
                
                var inputDescriptor = authorizationRequest.PresentationDefinition.InputDescriptors
                    .FirstOrDefault(x => x.Id == selectedCredentials[credential].InputDescriptorId);

                var presentationFormat = await _sdJwtVcHolderService.CreatePresentation((SdJwtRecord) selectedCredentials[credential].Credential, GetDisclosureNamesFromInputDescriptor(inputDescriptor), authorizationRequest.ClientId, authorizationRequest.Nonce);
                //var presentationFormat = await _sdJwtVcHolderService.CreateSdJwtPresentationFormatAsync(inputDescriptor, ((SdJwtRecord) selectedCredentials[credential].Credential).Id);
                vpToken.Add(presentationFormat);
                
                var descriptorMap = new DescriptorMap
                {
                    Format = "vc+sd-jwt",
                    Path = "$[" + credential + "]",
                    InputDescriptorId = selectedCredentials[credential].InputDescriptorId,
                    PathNested = null
                };
                descriptorMaps.Add(descriptorMap);
            }
            
            var presentationSubmission = await _pexService.CreatePresentationSubmission(authorizationRequest.PresentationDefinition, descriptorMaps.ToArray());
            var authorizationResponse = await _oid4VpClientService.CreateAuthorizationResponse(vpToken.ToArray(), presentationSubmission);
            
            return authorizationResponse;
        }


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
            
            if (authorizationRequest.ResponseMode != "direct_post") 
                return false;
            
            if (!(authorizationRequest.ClientIdScheme == "x509_san_dns" || authorizationRequest.ClientIdScheme == "verifier_attestation")) 
                return false;

            return true;
        }
    }
}
