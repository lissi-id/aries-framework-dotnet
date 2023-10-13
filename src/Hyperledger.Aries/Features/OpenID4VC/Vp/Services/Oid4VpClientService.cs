using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    /// <inheritdoc />
    public class Oid4VpClientService : IOid4VpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPexService _pexService;

        public Oid4VpClientService(
            IHttpClientFactory httpClientFactory,
            IPexService pexService)
        {
            _httpClientFactory = httpClientFactory;
            _pexService = pexService;
        }

        /// <inheritdoc />
        public async Task<AuthorizationRequest> ProcessAuthorizationRequest(Uri authorizationRequestUri)
        {
            
            var authorizationRequest = new AuthorizationRequest();
            
            var request = authorizationRequestUri.GetQueryParam("request_uri");
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
                authorizationRequest = AuthorizationRequest.ParseFromUri(authorizationRequestUri);
            }

            if (authorizationRequest == null) 
                throw new InvalidOperationException("Couldn't not parse Authorization Request Url");

            return authorizationRequest;
        }
        
        /// <inheritdoc />
        public async Task<AuthorizationResponse> CreateAuthorizationResponse(AuthorizationRequest authorizationRequest, (string inputDescriptorId, string presentation)[] presentationMap)
        {
            var descriptorMaps = new List<DescriptorMap>();
            var vpToken = new List<string>();
            for (var index = 0; index < presentationMap.Length; index++)
            {
                vpToken.Add(presentationMap[index].presentation);
                
                var descriptorMap = new DescriptorMap
                {
                    Format = "vc+sd-jwt",
                    Path = "$[" + index + "]",
                    Id = presentationMap[index].inputDescriptorId,
                    PathNested = null
                };
                descriptorMaps.Add(descriptorMap);
            }

            var presentationSubmission = await _pexService.CreatePresentationSubmission(authorizationRequest.PresentationDefinition, descriptorMaps.ToArray());
            
            return new AuthorizationResponse
            {
                PresentationSubmission = JsonConvert.SerializeObject(presentationSubmission),
                VpToken = vpToken.ToString()
            };
        }
        
    }
}
