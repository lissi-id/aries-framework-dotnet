using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.OpenID4VC.JWT.Services;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialResponse;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Services.IssuanceService;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.OpenId4Vc.Vci.Services
{
    public class DefaultOidIssuanceServiceTests
    {
        private DefaultOidIssuanceService _issuanceService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        private readonly Mock<IJwtFactory> _jwtFactoryMock = new Mock<IJwtFactory>();

        private readonly OidIssuerMetadata _oidIssuerMetadata = new OidIssuerMetadata{
            CredentialIssuer = "https://issuer.io",
            CredentialEndpoint = "https://issuer.io/credential",
            CredentialsSupported = new[]
            {
                new OidCredentialMetadata
                {
                    Format = "SimpleCredential",
                    Type = "SimpleCredentialType",
                    CredentialSubject = new Dictionary<string, Display>()
                }
            }
        };

        [Fact]
        public async Task CanGetIssuerMetadataAsync()
        {
            // Arrange
            var responseContent = JsonConvert.SerializeObject(_oidIssuerMetadata);
            SetupHttpClientSequence(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

            var expectedMetadata = JsonConvert.DeserializeObject<OidIssuerMetadata>(responseContent);

            // Act
            var actualMetadata = await _issuanceService.FetchIssuerMetadataAsync("https://issuer.io");

            // Assert
            actualMetadata.Should().BeEquivalentTo(expectedMetadata);
        }

        [Fact]
        public async Task CanRequestCredentialAsync()
        {
            // Arrange
            var expectedCredentialResponse = new OidCredentialResponse
            {
                Credential = "sampleCredential",
                Format = "SimpleCredential",
            };

            const string jwtMock = "mockJwt";
            _jwtFactoryMock.Setup(j => j.CreateJwtFromHardwareKeyAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(jwtMock);

            var credentialResponseContent = JsonConvert.SerializeObject(expectedCredentialResponse);
            SetupHttpClientSequence(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(credentialResponseContent)
                });

            var mockTokenResponse = new TokenResponse
            {
                AccessToken = "sampleAccessToken",
                TokenType = "bearer",
                ExpiresIn = 3600,
                CNonce = "sampleCNonce",
                CNonceExpiresIn = 3600
            };

            // Act
            var actualCredentialResponse = await _issuanceService.RequestCredentialAsync(
                "https://issuer.io",
                "sampleClientNonce",
                "SimpleCredentialType",
                mockTokenResponse
            );

            // Assert
            actualCredentialResponse.Should().BeEquivalentTo(expectedCredentialResponse);
        }

        [Fact]
        public async Task CanRequestTokenAsync()
        {
            // Arrange
            var expectedTokenResponse = new TokenResponse
            {
                AccessToken = "sampleAccessToken",
                TokenType = "bearer",
                ExpiresIn = 3600,
                CNonce = "sampleCNonce",
                CNonceExpiresIn = 3600
            };

            const string authServerMetadata =
                "{\"issuer\": \"https://issuer.io\", \"token_endpoint\": \"https://issuer.io/token\", \"token_endpoint_auth_methods_supported\": [\"urn:ietf:params:oauth:client-assertion-type:verifiable-presentation\"], \"response_types_supported\": [\"urn:ietf:params:oauth:grant-type:pre-authorized_code\"]}\n";

            var tokenResponseContent = JsonConvert.SerializeObject(expectedTokenResponse);

            SetupHttpClientSequence(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(authServerMetadata)
                },
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponseContent)
                });

            // Act
            var actualTokenResponse =
                await _issuanceService.RequestTokenAsync(_oidIssuerMetadata, "samplePreAuthorizedCode");

            // Assert
            actualTokenResponse.Should().BeEquivalentTo(expectedTokenResponse);
        }

        private void SetupHttpClientSequence(params HttpResponseMessage[] responses)
        {
            var responseQueue = new Queue<HttpResponseMessage>(responses);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => responseQueue.Dequeue());

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _issuanceService = new DefaultOidIssuanceService(_httpClientFactoryMock.Object, _jwtFactoryMock.Object);
        }
    }
}
