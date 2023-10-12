using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    public class Oid4VpClientService : IOid4VpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Oid4VpClientService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task<AuthorizationRequest> ProcessAuthorizationRequest(string authorizationRequestUrl)
        {
            var uri = new Uri(authorizationRequestUrl);
            
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
                //TODO: Add functionality to parse presentation_definition_uri parameter
                authorizationRequest = AuthorizationRequest.ParseFromUri(uri);
            }

            if (authorizationRequest == null) 
                throw new InvalidOperationException("Couldn't not parse Authorization Request Url");

            return authorizationRequest;
        }
        
        /// <inheritdoc />
        public AuthorizationResponse CreateAuthorizationResponse(string[] vpToken, PresentationSubmission presentationSubmission)
        {
            return new AuthorizationResponse
            {
                PresentationSubmission = JsonConvert.SerializeObject(presentationSubmission),
                VpToken = vpToken.ToString()
            };
        }
        
    }
}
