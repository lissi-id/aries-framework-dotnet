using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Services
{
    /// <inheritdoc />
    public class Oid4VpClientCore : IOid4VpClientCore
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPexService _pexService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Oid4VpClientCore" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The http client factory to create http clients.</param>
        /// <param name="pexService">The service responsible for presentation exchange protocol operations.</param>
        public Oid4VpClientCore(
            IHttpClientFactory httpClientFactory,
            IPexService pexService)
        {
            _httpClientFactory = httpClientFactory;
            _pexService = pexService;
        }

        /// <inheritdoc />
        public async Task<AuthorizationRequest> ProcessAuthorizationRequest(HaipAuthorizationRequestUri haipAuthorizationRequestUri)
        {
            var authorizationRequest = new AuthorizationRequest();

            if (!String.IsNullOrEmpty(haipAuthorizationRequestUri.RequestUri))
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(haipAuthorizationRequestUri.RequestUri);
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    authorizationRequest = AuthorizationRequest.ParseFromJwt(content);
                }
            }
            else
            {
                authorizationRequest = AuthorizationRequest.ParseFromUri(haipAuthorizationRequestUri.Uri);
            }

            if (authorizationRequest == null) 
                throw new InvalidOperationException("Could not parse Authorization Request Url");

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
                VpToken = JsonConvert.SerializeObject(vpToken),
                State = authorizationRequest.State
            };
        }
    }
}
