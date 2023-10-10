using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public class Oid4VpClientService : IOid4VpClientService
    {
        private readonly IPexService _pexService;
        private readonly ISdJwtVcHolderService _sdJwtVcHolderService;
        private readonly IHttpClientFactory _httpClientFactory;

        public Oid4VpClientService(
            IPexService pexService,
            ISdJwtVcHolderService sdJwtVcHolderService,
            IHttpClientFactory httpClientFactory)
        {
            _pexService = pexService;
            _sdJwtVcHolderService = sdJwtVcHolderService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl)
        {
            var json = "%7B%22response_type%22:%22vp_token%22,%22client_id%22:%22https://nc-sd-jwt.lambda.d3f.me/index.php/apps/ssi_login/oidc/callback%22,%22redirect_uri%22:%22https://nc-sd-jwt.lambda.d3f.me/index.php/apps/ssi_login/oidc/callback%22,%22nonce%22:%2287554784260280280442092184171274132458%22,%22presentation_definition%22:%7B%22id%22:%224dd1c26a-2f46-43ae-a711-70888c93fb4f%22,%22input_descriptors%22:%5B%7B%22id%22:%22NextcloudCredential%22,%22format%22:%7B%22vc+sd-jwt%22:%7B%22proof_type%22:%5B%22JsonWebSignature2020%22%5D%7D%7D,%22constraints%22:%7B%22limit_disclosure%22:%22required%22,%22fields%22:%5B%7B%22path%22:%5B%22$.type%22%5D,%22filter%22:%7B%22type%22:%22string%22,%22const%22:%22VerifiedEMail%22%7D%7D,%7B%22path%22:%5B%22$.credentialSubject.email%22%5D%7D%5D%7D%7D%5D%7D%7D";
            authorizationRequestUrl = "openid4vp://?response_type=vp_token&client_id=https%3A%2F%2Fclient.example.org%2Fcb&redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb&presentation_definition=" + json;

            var uri = new Uri(authorizationRequestUrl);
            //TODO: Update to HAIP later
            if (uri.Scheme != "openid4vp") 
                throw new InvalidOperationException("Only openId4VP scheme is supported");
            
            var authorizationRequest = new AuthorizationRequest();
            
            var request = uri.GetQueryParam("request_uri");
            if (!String.IsNullOrEmpty(request))
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(request);
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    authorizationRequest = AuthorizationRequest.ParseFromJwt(content);
                }
            }
            else
            {
                authorizationRequest = AuthorizationRequest.ParseFromUri(uri);
            }

            if (authorizationRequest == null) 
                throw new InvalidOperationException("Couldn't not parse Authorization Request Url");

            //TODO: check for HAIP conformity ==> IsAuthorizationRequestHaipConform()

            return authorizationRequest;
        }

        public Task<AuthorizationResponse> CreateAuthorizationResponse(AuthorizationRequest authorizationRequest, SelectedCredential[] selectedCredentials,
            PresentationSubmission presentationSubmission)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<AuthorizationResponse> CreateAuthorizationResponse(string[] vpToken, PresentationSubmission presentationSubmission)
        {
            
            
            return new AuthorizationResponse
            {
                PresentationSubmission = JsonConvert.SerializeObject(presentationSubmission),
                VpToken = vpToken.ToString()
            };
        }
        
    }
}
