using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public class Oid4VpClientService : IOid4VpClientService
    {
        private readonly IPexService _pexService;
        private readonly ISdJwtVcHolderService _sdJwtVcHolderService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAgentProvider _agentProvider;
        private readonly IWalletRecordService _recordService;

        public Oid4VpClientService(
            IPexService pexService,
            ISdJwtVcHolderService sdJwtVcHolderService,
            IHttpClientFactory httpClientFactory,
            IAgentProvider agentProvider,
            IWalletRecordService recordService)
        {
            _pexService = pexService;
            _sdJwtVcHolderService = sdJwtVcHolderService;
            _httpClientFactory = httpClientFactory;
            _agentProvider = agentProvider;
            _recordService = recordService;
        }

        public async Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl)
        {
            var uri = new Uri(authorizationRequestUrl);
            //if (uri.Scheme != "haip") throw new InvalidOperationException("Only haip scheme is supported");
            
            if (!uri.HasQueryParam("request_uri"))
                throw new InvalidOperationException("Haip only supports request_uri");
            
            AuthorizationRequest? authorizationRequest = null;
            
            var request = uri.GetQueryParam("request_uri");
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                authorizationRequest = await ParseFromJwt(content);
            }

            //if (!IsAuthorizationRequestHaipConform(authorizationRequest))
                //throw new InvalidOperationException("Authorization request is not haip conform");
            
            return authorizationRequest;
        }

        public async Task SendAuthorizationResponse(SelectedCredential[] selectedCredentials, string authorizationRequestRecordId)
        {
            var context = await _agentProvider.GetContextAsync();
            var authRecord =
                await _recordService.GetAsync<AuthorizationRequestRecord>(context.Wallet, authorizationRequestRecordId);

            List<CredentialDescriptor>? credentialDescriptors = null;
            List<string>? vpToken = null;
            for (var credential = 0; credential < selectedCredentials.Length; credential++)
            {
                //TODO: Other Credential Types
                if (selectedCredentials[credential].Credential.GetType().BaseType != typeof(SdJwtRecord)) 
                    throw new InvalidOperationException("only SD-JWT credentials are supported");
                
                var inputDescriptor = authRecord.AuthorizationRequest.PresentationDefinition.InputDescriptors.FirstOrDefault(x =>
                    x.Id == selectedCredentials[credential].InputDescriptorId);
                    
                var presentationFormat = await _sdJwtVcHolderService.CreateSdJwtPresentationFormat(inputDescriptor, ((SdJwtRecord) selectedCredentials[credential].Credential).Id);
                vpToken.Add(presentationFormat);
                    
                var credentialDescriptor = new CredentialDescriptor
                {
                    CredentialId = ((SdJwtRecord) selectedCredentials[credential].Credential).Id,
                    Format = "jwt_vp_json", //???
                    Path = "$[" + credential + "]",
                    InputDescriptorId = selectedCredentials[credential].InputDescriptorId,
                    PathNested = null
                };
                credentialDescriptors.Add(credentialDescriptor);
            }

            var presentationSubmission = await _pexService.CreatePresentationSubmission(authRecord.AuthorizationRequest.PresentationDefinition, credentialDescriptors.ToArray());
            
            await PostAuthorizationResponse(authRecord.AuthorizationRequest, vpToken.ToString(), presentationSubmission.ToString());
            
            // Call SdJwtService.CreateSdJwtPresentationFormat(inputDesciptor, credentialId) for each SD JWT credential
            // Call IPexService.CreatePresentationSubmission(presentationDefinition, credentials)
            // --> send Response
        }
        
        private async Task PostAuthorizationResponse(AuthorizationRequest authorizationRequest, string vpToken, string presentationSubmission)
        {
            var content = new List<KeyValuePair<string, string>>();
            content.Add(new KeyValuePair<string, string>("vp_token", vpToken));
            content.Add(new KeyValuePair<string, string>("presentation_submission", presentationSubmission));
            
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(authorizationRequest.RedirectUri),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(content)
            };

            var httpClient = _httpClientFactory.CreateClient();
            await httpClient.SendAsync(request);
        }
        
        private async Task<AuthorizationRequest> ParseFromJwt(string jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadToken(jwt) as JwtSecurityToken;
            var json = token?.Payload.SerializeToJson() ?? "";

            var authorizationRequest = JsonConvert.DeserializeObject<AuthorizationRequest>(json);
                    
            authorizationRequest.PresentationDefinition =
                await _pexService.ParsePresentationDefinition(authorizationRequest
                    .PresentationDefinitionAsString.ToString());

            return authorizationRequest;
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
