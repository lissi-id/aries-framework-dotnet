using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.OpenId4Vc.KeyStore.Services;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Authorization;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialResponse;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential.Attributes;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Services.Oid4VciService;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.OpenId4Vc.Vci.Services
{
    public class DefaultOid4VciServiceTests
    {
        private DefaultOid4VciService _oid4VciService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        private readonly Mock<IKeyStore> _jwtFactoryMock = new Mock<IKeyStore>();

        private readonly OidIssuerMetadata _oidIssuerMetadata = new OidIssuerMetadata{
            CredentialIssuer = "https://issuer.io",
            CredentialEndpoint = "https://issuer.io/credential",
            CredentialsSupported = new[]
            {
                new OidCredentialMetadata
                {
                    Format = "SimpleCredential",
                    Type = "SimpleCredentialType",
                    CredentialSubject = new Dictionary<string, OidCredentialSubjectAttribute>()
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
            var actualMetadata = await _oid4VciService.FetchIssuerMetadataAsync(new Uri("https://issuer.io"));

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
            _jwtFactoryMock.Setup(j => j.CreateProofOfPossessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            var actualCredentialResponse = await _oid4VciService.RequestCredentialAsync(
                "https://issuer.io",
                "sampleClientNonce",
                "SimpleCredentialType",
                mockTokenResponse
            );

            // Assert
            actualCredentialResponse.Item1.Should().BeEquivalentTo(expectedCredentialResponse);
            actualCredentialResponse.Item2.Should().NotBeNullOrEmpty();
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
                await _oid4VciService.RequestTokenAsync(_oidIssuerMetadata, "samplePreAuthorizedCode");

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

            _oid4VciService = new DefaultOid4VciService(_httpClientFactoryMock.Object, _jwtFactoryMock.Object);
        }
    }
}
