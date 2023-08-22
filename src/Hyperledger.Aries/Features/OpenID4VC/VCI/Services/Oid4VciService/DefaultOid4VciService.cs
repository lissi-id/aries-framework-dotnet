#nullable enable

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.OpenID4VC.JWT.Services;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialResponse;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vci.Services.Oid4VciService
{
    /// <inheritdoc />
    public class DefaultOid4VciService : IOid4VciService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultOid4VciService" /> class.
        /// </summary>
        /// <param name="httpClientFactory">
        ///     The factory to create instances of <see cref="HttpClient" />. Used for making HTTP
        ///     requests.
        /// </param>
        /// <param name="jwtFactory">The factory responsible for creating JWT tokens.</param>
        public DefaultOid4VciService(
            IHttpClientFactory httpClientFactory,
            IJwtFactory jwtFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jwtFactory = jwtFactory;
        }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJwtFactory _jwtFactory;

        /// <inheritdoc />
        public async Task<OidIssuerMetadata> FetchIssuerMetadataAsync(Uri endpoint)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var metadataUrl = new Uri(endpoint, ".well-known/openid-credential-issuer");

            var response = await httpClient.GetAsync(metadataUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<OidIssuerMetadata>(responseString)
                       ?? throw new InvalidOperationException(
                           "Failed to deserialize the issuer metadata. JSON: " +
                           responseString);

            throw new HttpRequestException(
                $"Failed to get Issuer metadata. Status code is {response.StatusCode} with message {responseString}");
        }

        /// <inheritdoc />
        public async Task<(OidCredentialResponse, string)> RequestCredentialAsync(
            string credentialIssuer,
            string clientNonce,
            string type,
            TokenResponse tokenResponse)
        {
            var keyAlias = Guid.NewGuid().ToString();
            
            var jwt = await _jwtFactory.CreateJwtFromHardwareKeyAsync(
                credentialIssuer, clientNonce, keyAlias);

            var credentialRequest = BuildCredentialRequest(jwt, type);
            var responseData = await SendCredentialRequest(credentialIssuer, tokenResponse, credentialRequest);

            var responseString = await responseData.Content.ReadAsStringAsync();
            if (!responseData.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Failed to request Credential. Status Code is {responseData.StatusCode} with message {responseString}");

            var credentialResponse = JsonConvert.DeserializeObject<OidCredentialResponse>(responseString)
                                     ?? throw new InvalidOperationException(
                                         "Failed to deserialize the credential response. JSON: " +
                                         responseString);

            if (credentialResponse.Credential == null)
                throw new InvalidOperationException("Credential in response is null.");

            return (credentialResponse, keyAlias);
        }

        /// <inheritdoc />
        public async Task<TokenResponse> RequestTokenAsync(OidIssuerMetadata metadata, string preAuthorizedCode,
            string? pin = null)
        {
            var authServer = await GetAuthorizationServerMetadata(metadata);
            return await GetRequestTokenAsync(preAuthorizedCode, authServer, pin);
        }

        private static OidCredentialRequest BuildCredentialRequest(string jwt, string type)
        {
            return new OidCredentialRequest
            {
                Format = "vc+sd-jwt",
                Type = type,
                Proof = new OidProofOfPossession
                {
                    ProofType = "jwt",
                    Jwt = jwt
                }
            };
        }

        private static Task<FormUrlEncodedContent> CreateRequestToken(string preAuthorizedCode, string? pin)
        {
            var tokenRequest = new TokenRequest
            {
                GrantType = "urn:ietf:params:oauth:grant-type:pre-authorized_code",
                PreAuthorizedCode = preAuthorizedCode
            };

            if (!string.IsNullOrEmpty(pin))
                tokenRequest.UserPin = pin;

            return Task.FromResult(tokenRequest.ToFormUrlEncoded());
        }

        private async Task<AuthorizationServerMetadata> FetchAuthorizationServerMetadataAsync(string endpointUrl)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(endpointUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<AuthorizationServerMetadata>(responseString)
                       ?? throw new InvalidOperationException(
                           "Failed to deserialize the authorization server metadata. JSON: " + responseString);

            throw new HttpRequestException(
                $"Failed to get authorization server metadata. Status Code is: {response.StatusCode} with message {responseString}");
        }

        private async Task<AuthorizationServerMetadata> GetAuthorizationServerMetadata(OidIssuerMetadata metadata)
        {
            string endpointUrl;
            if (!string.IsNullOrEmpty(metadata.AuthorizationServer))
            {
                endpointUrl = metadata.AuthorizationServer;
            }
            else
            {
                var credentialIssuerUrl = new Uri(metadata.CredentialIssuer);
                endpointUrl = new Uri(credentialIssuerUrl, "/.well-known/oauth-authorization-server").ToString();
            }

            return await FetchAuthorizationServerMetadataAsync(endpointUrl);
        }

        private async Task<TokenResponse> GetRequestTokenAsync(
            string preAuthorizedCode,
            AuthorizationServerMetadata? authorizationServer,
            string? pin = null)
        {
            var formUrlEncodedRequest = await CreateRequestToken(preAuthorizedCode, pin);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(authorizationServer?.TokenEndpoint, formUrlEncodedRequest);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TokenResponse>(responseString) ??
                       throw new InvalidOperationException("Failed to deserialize the token response. JSON: " +
                                                           responseString);

            throw new HttpRequestException(
                $"Failed to get token. Status Code is {response.StatusCode} with message {responseString}");
        }

        private async Task<HttpResponseMessage> SendCredentialRequest(
            string credentialIssuer,
            TokenResponse tokenResponse,
            OidCredentialRequest credentialRequest)
        {
            var requestData = new StringContent(credentialRequest.ToJson(), Encoding.UTF8, "application/json");

            var httpClientWithAuth = _httpClientFactory.CreateClient();
            httpClientWithAuth.DefaultRequestHeaders.Add("Authorization",
                $"{tokenResponse.TokenType} {tokenResponse.AccessToken}");

            return await httpClientWithAuth.PostAsync(Url.Combine(credentialIssuer, "/credential"), requestData);
        }
    }
}
