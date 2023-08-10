using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.OpenID4Common.Events;
using Hyperledger.Aries.Features.OpenID4Common.Records;
using Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Helpers;
using Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models;
using Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models.PresentationExchange;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SD_JWT.Abstractions;
using SD_JWT.Models;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation
{
    public class OpenId4VpClient : IOpenId4VpClient
    {
        public OpenId4VpClient(
            IWalletRecordService recordService,
            IEventAggregator eventAggregator, 
            IHolder holder)
        {
            _recordService = recordService;
            _eventAggregator = eventAggregator;
            _holder = holder;
            _httpClient = new HttpClient();
        }
        
        private readonly HttpClient _httpClient;
        private readonly IWalletRecordService _recordService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IHolder _holder;
        
        public async Task<string> ProcessAuthenticationRequestUrl(IAgentContext agentContext, string url)
        {
            var uri = new Uri(url);
            AuthorizationRequest? authorizationRequest = null;
            if (uri.HasQueryParam("request_uri"))
            {
                var request = uri.GetQueryParam("request_uri");
                var response = await _httpClient.GetAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    authorizationRequest = AuthorizationRequest.ParseFromBase64Url(content);
                    
                    // var split = content.Split(".")[1];
                    // authorizationRequest = AuthorizationRequest.ParseFromBase64Url(split);
                }
            }
            else
            {
                //authorizationRequest = AuthorizationRequest.ParseFromUri(uri);
                //authorizationRequest = "{\"response_type\":\"vp_token\",\"client_id\":\"https://verifier.lissi.io/callback\",\"response_mode\":\"direct_post\",\"redirect_uri\":\"https://verifier.lissi.io/callback\",\"nonce\":\"22456608037449564223\",\"presentation_definition\":{\"id\":\"d76c51b7-ea90-49bb-8368-6b3d194fc131\",\"input_descriptors\":[{\"id\":\"ExampleCredential\",\"format\":{\"verifiable-credential+sd-jwt\":{\"proof_type\":[\"JsonWebSignature2020\"]}},\"constraints\":{\"limit_disclosure\":\"required\",\"fields\":[{\"path\":[\"$.type\"],\"filter\":{\"type\":\"string\",\"const\":\"SimpleCredential\"}},{\"path\":[\"$.first_name\"]}]}}]}}";
            }
            
            if (authorizationRequest == null)
                throw new NullReferenceException("Unable to process OpenId url");

            var record = new OpenId4VpRecord
            {
                Id = Guid.NewGuid().ToString(),
                PresentationDefinition = authorizationRequest.PresentationDefinition.ToString(),
                ResponseMode = authorizationRequest.ResponseMode,
                Nonce = authorizationRequest.Nonce,
                RedirectUri = authorizationRequest.RedirectUri,
                ClientId = authorizationRequest.ClientId
            };
            
            await _recordService.AddAsync(agentContext.Wallet, record);
            
            _eventAggregator.Publish(new NewPresentationRequestEvent()
            {
                RecordId = record.Id
            });

            return record.Id;
        }
        
        public async Task<string?> GenerateAuthenticationResponse(IAgentContext agentContext, string authRecordId, string credRecordId, RequestedCredentials requestedCredentials)
        {
            var authRecord = await _recordService.GetAsync<OpenId4VpRecord>(agentContext.Wallet, authRecordId);
            var sdJwtRecord = await _recordService.GetAsync<SdJwtRecord>(agentContext.Wallet, credRecordId);

            var vpToken = CreateVpToken(sdJwtRecord, authRecord, requestedCredentials);
            // Todo: Create presentation submission dynamically
            var sdJwtDoc = new SdJwtDoc(sdJwtRecord.CombinedIssuance);
            var presentationSubmission = CreatePresentationSubmission(authRecord, sdJwtDoc);
            if (authRecord.ResponseMode == "direct_post")
            {
                //await SendAuthorizationResponse(authRecord, vpToken, JsonConvert.SerializeObject(presentationSubmission));
                return null;
            }

            return null;
            // else
            // {
            //     var callbackUrl = PrepareAuthorizationResponse(authRecord!, vpToken,
            //         JsonConvert.SerializeObject(presentationSubmission));
            //         
            //     return callbackUrl;
            // }
        }

        public async Task<OpenId4VpRecord> GetOpenId4VpRecordAsync(IAgentContext agentContext, string recordId)
        {
            var record = await _recordService.GetAsync<OpenId4VpRecord>(agentContext.Wallet, recordId);

            if (record == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "OpenId4VciRecord record not found");

            return record;
        }

        public Task<List<OpenId4VpRecord>> ListOpenId4VpRecordAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100, int skip = 0) => 
            _recordService.SearchAsync<OpenId4VpRecord>(agentContext.Wallet, query, null, count, skip);

        private string PrepareAuthorizationResponse(OpenId4VpRecord authorizationRequest, object vpToken, string presentation_submission)
        {
            var redirectUri = new Uri(authorizationRequest.RedirectUri);

            var callbackUri = new UriBuilder();
            callbackUri.Scheme = redirectUri.Scheme;
            callbackUri.Host = redirectUri.Host;
            callbackUri.Port = redirectUri.Port;
            callbackUri.Path = redirectUri.PathAndQuery.Contains("?") ? redirectUri.PathAndQuery.Split("?").First() : redirectUri.PathAndQuery;
            callbackUri.Query = $"response_type=vp_token&presentation_submission={presentation_submission}&vp_token={vpToken}";

            return Uri.EscapeUriString(callbackUri.Uri.ToString());
        }
        
        private async Task SendAuthorizationResponse(OpenId4VpRecord authorizationRequest, object vpToken, string presentationSubmission)
        {
            var content = new List<KeyValuePair<string, string>>();
            content.Add(new KeyValuePair<string, string>("vp_token", vpToken.ToString()));
            content.Add(new KeyValuePair<string, string>("presentation_submission", presentationSubmission));
            
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(authorizationRequest.RedirectUri),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(content)
            };

            await _httpClient.SendAsync(request);
        }
        
        private object CreateVpToken(SdJwtRecord sdJwtRecord, OpenId4VpRecord vpRecord, RequestedCredentials requestedCredentials)
        {
            var sdJwtDoc = new SdJwtDoc(sdJwtRecord.CombinedIssuance);
            var disclosuresToPresent = new List<string>();
            foreach (var item in requestedCredentials.RequestedAttributes)
            {
                disclosuresToPresent.Add(item.Key);
            }
            return _holder.CreatePresentation(sdJwtDoc, disclosuresToPresent.ToArray(), "my_ec_key_alias", vpRecord.Nonce, vpRecord.ClientId);
        }

        private static object CreatePresentationSubmission(OpenId4VpRecord openIdRecord, SdJwtDoc sdJwtDoc)
        {
            var request = PresentationDefinition.FromJson(openIdRecord.PresentationDefinition);
            var jo = sdJwtDoc.GetDisclosuresAsJsonRepresentation();
            return new
            {
                id = Guid.NewGuid().ToString(),
                definition_id = request.Id,
                descriptor_map = new[]
                {
                    new
                    {
                        id = JObject.Parse(jo).SelectToken("$.type"),
                        format = "vp+sd-jwt",
                        path = "$"
                    }
                }
            };
        }
    }
}
