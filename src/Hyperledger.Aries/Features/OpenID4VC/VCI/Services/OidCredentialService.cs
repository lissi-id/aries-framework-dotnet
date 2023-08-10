using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.OpenID4VC.JWT;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialRequest;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Token;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SD_JWT.Abstractions;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Services;

/// <inheritdoc />
public class OidCredentialService : IOidCredentialService
{
    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    public OidCredentialService(
        IAgentProvider agentProvider,
        IJwtFactory jwtFactory,
        IHolder holder,
        IHttpClientFactory httpClientFactory,
        ILogger<OidCredentialService> logger,
        IWalletRecordService recordService)
    {
        _agentProvider = agentProvider;
        _jwtFactory = jwtFactory;
        _holder = holder;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _recordService = recordService;
    }

    private readonly IAgentProvider _agentProvider;
    private readonly IJwtFactory _jwtFactory;
    private readonly IHolder _holder;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OidCredentialService> _logger;
    private readonly IWalletRecordService _recordService;

    /// <inheritdoc />
    public async Task<OidIssuerMetadata> FetchIssuerMetadataAsync(string endpoint)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var metadataUrl = $"{endpoint}/.well-known/openid-credential-issuer";

        try
        {
            var response = await httpClient.GetAsync(metadataUrl);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OidIssuerMetadata>(result);
            }

            Debug.WriteLine($"HTTP Request failed: {response.StatusCode}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error fetching metadata: {e.Message}");
        }

        return null;
    }

    /// <inheritdoc />
    public Task GetAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task ListAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task RequestCredentialAsync(string credentialIssuer, string clientNonce, string type, TokenResponse tokenResponse)
    {
        var jwt 
            = await _jwtFactory.CreateProofOfPossessionJwtAsync(credentialIssuer, clientNonce);
        
        var proofOfPossession = new OidProofOfPossession("jwt", jwt);
        
        var credentialRequest = new OidCredentialRequest(
            "vc+sd-jwt",
            type,
            proofOfPossession);
        
        var requestData = new StringContent(credentialRequest.ToJson(), Encoding.UTF8, "application/json");
        HttpResponseMessage credHttpResponse;
        using (var httpClientWithAuth = new HttpClient())
        {
            httpClientWithAuth.DefaultRequestHeaders.Add("Authorization", tokenResponse.TokenType + " " + tokenResponse.AccessToken);
            credHttpResponse = await httpClientWithAuth.PostAsync(
                Url.Combine(credentialIssuer, "/credential"), requestData);
        }
            
        var credResponseString = await credHttpResponse.Content.ReadAsStringAsync();
        var credResponse = credResponseString.ToObject<OidCredentialResponse>();
        
        var credential = credResponse.Credential;
        var sdJwt = _holder.ReceiveCredential(credential);
        
        var disclosures = sdJwt.Disclosures.ToDictionary(
            disclosure => disclosure.Name, disclosure => disclosure.Value.ToString());

        var displayedAttributes = new Dictionary<string, Dictionary<string, string>>
        {
            { "en", disclosures }
        };

        var sdJwtRecord = new SdJwtRecord
        {
            Id = Guid.NewGuid().ToString(),
            IssuerId = credentialIssuer,
            DisplayedAttributes = displayedAttributes,
            Claims = disclosures,
            CombinedIssuance = credential,
            IssuerName = credentialIssuer,
            BackgroundColor = "#333333",
        };

        var context = await _agentProvider.GetContextAsync();
        await _recordService.AddAsync(context.Wallet, sdJwtRecord);

        if (credHttpResponse.IsSuccessStatusCode)
        {
            Debug.WriteLine("Success");
        }
        else
        {
            throw new Exception($"Status Code is {credHttpResponse.StatusCode} with message {credResponseString}");
        }
    }

    /// <inheritdoc />
    public async Task<TokenResponse> RequestTokenAsync(OidIssuerMetadata metadata, string preAuthorizedCode)
    {
        var authServer = await GetAuthorizationServerMetadata(metadata);
        var tokenResponseString = await RequestTokenAsync(preAuthorizedCode, authServer);
        return JsonConvert.DeserializeObject<TokenResponse>(tokenResponseString);
    }

    private static async Task<FormUrlEncodedContent> CreateRequestToken(string preAuthorizedCode)
    {
        var tokenValues = new Dictionary<string, string>
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:pre-authorized_code" },
            { "pre-authorized_code", preAuthorizedCode },
            { "user_pin", "2360" }
        };

        // if (string.IsNullOrEmpty(attestation) == false)
        //     tokenValues.Add("wallet_attestation", attestation);
        // if (string.IsNullOrEmpty(userPIN) == false)
        //     tokenValues.Add("user_pin", userPIN);

        var token = new FormUrlEncodedContent(tokenValues);
        Debug.WriteLine(await token.ReadAsStringAsync());
        return token;
    }

    private async Task<string> FetchAuthorizationServerMetadataAsync(string credentialIssuer)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var credentialIssuerUrl = new Uri(credentialIssuer);
        var authorizationServerUrl =
            new Uri(credentialIssuerUrl, "/.well-known/oauth-authorization-server");

        var response = await httpClient.GetAsync(authorizationServerUrl);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        throw new Exception($"Failed to get authorization server, Status Code: {response.StatusCode}");
    }

    private async Task<AuthorizationServer> GetAuthorizationServerMetadata(OidIssuerMetadata metadata)
    {
        string result;
        if (!string.IsNullOrEmpty(metadata.AuthorizationServer))
            result = metadata.AuthorizationServer;
        else
            result = await FetchAuthorizationServerMetadataAsync(metadata.CredentialIssuer);

        return JsonConvert.DeserializeObject<AuthorizationServer>(result);
    }

    private async Task<string> RequestTokenAsync(string preAuthorizedCode, AuthorizationServer authorizationServer)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var token = await CreateRequestToken(preAuthorizedCode);
        var tokenHttpResponse = await httpClient.PostAsync(authorizationServer.TokenEndpoint, token);

        if (tokenHttpResponse.IsSuccessStatusCode)
            return await tokenHttpResponse.Content.ReadAsStringAsync();

        throw new Exception($"Failed to get token, Status Code: {tokenHttpResponse.StatusCode}");
    }
}
