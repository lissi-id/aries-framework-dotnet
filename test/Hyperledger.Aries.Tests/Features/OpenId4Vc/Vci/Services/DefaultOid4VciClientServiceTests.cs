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
    public class DefaultOid4VciClientServiceTests
    {
        private DefaultOid4VciClientService _oid4VciClientService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        private readonly Mock<IKeyStore> _keyStoreMock = new Mock<IKeyStore>();

        private readonly OidIssuerMetadata _oidIssuerMetadata = new OidIssuerMetadata{
            CredentialIssuer = "https://issuer.io",
            CredentialEndpoint = "https://issuer.io/credential",
            CredentialsSupported = new List<OidCredentialMetadata>
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
            const string responseContent = "{\"credential_issuer\":\"https://issuer.io\",\"credential_endpoint\":\"https://issuer.io/credential\",\"display\":[{\"name\":\"Aussteller\",\"locale\":\"de-DE\"},{\"name\":\"Issuer\",\"locale\":\"en-US\"}],\"credentials_supported\":[{\"id\":\"SimpleCredential\",\"format\":\"vc+sd-jwt\",\"type\":\"SimpleCredentialType\",\"cryptographic_binding_methods_supported\":[\"jwk\"],\"cryptographic_suites_supported\":[\"ES256\"],\"display\":[{\"name\":\"Einfacher Nachweis\",\"locale\":\"de-DE\",\"logo\":{\"url\":\"https://logo.png\",\"alt_text\":\"Logo\"},\"background_color\":\"#111111\",\"text_color\":\"#FFFFFF\"},{\"name\":\"Simple Credential\",\"locale\":\"en-US\",\"logo\":{\"url\":\"https://logo.png\",\"alt_text\":\"Logo\"},\"background_color\":\"#111111\",\"text_color\":\"#FFFFFF\"}],\"credentialSubject\":{\"first_name\":{\"display\":[{\"name\":\"First Name\",\"locale\":\"en-US\"},{\"name\":\"Vorname\",\"locale\":\"de-DE\"}]},\"last_name\":{\"display\":[{\"name\":\"Last Name\",\"locale\":\"en-US\"},{\"name\":\"Nachname\",\"locale\":\"de-DE\"}]}}}]}";
            SetupHttpClientSequence(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });
            
            var expectedMetadata = JsonConvert.DeserializeObject<OidIssuerMetadata>(responseContent);
            
            // Act
            var actualMetadata = await _oid4VciClientService.FetchIssuerMetadataAsync(new Uri("https://issuer.io"));

            // Assert
            actualMetadata.Should().BeEquivalentTo(expectedMetadata);
        }

        [Fact]
        public async Task CanRequestCredentialAsync()
        {
            // Arrange
            const string jwtMock = "mockJwt";
            const string keyId = "keyId";
            _keyStoreMock.Setup(j => j.CreateProofOfPossessionAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((jwtMock, keyId));
            
            const string credentialResponse = "{\"format\":\"vc+sd-jwt\",\"credential\":\"eyJhbGciOiAiRVMyNTYifQ.eyJfc2QiOlsiT0dfT2lCMk5ZS0JzTVhIOFVVb2luREhUT1h5VER1Z3JPdE94RFI2NF9ZcyIsIlQzbHRYQUFtODNJTXRUYkRTb1J2d1g2Tk10em1scV9ZWG9Vd1EwZDY0NEUiXSwiaXNzIjoiaHR0cHM6Ly9pc3N1ZXIuaW8vIiwiaWF0IjoxNTE2MjM5MDIyLCJ0eXBlIjoiU2ltcGxlQ3JlZGVudGlhbCIsImV4cCI6MTUxNjI0NzAyMiwiX3NkX2FsZyI6InNoYS0yNTYiLCJjbmYiOnsiandrIjp7Imt0eSI6IkVDIiwiY3J2IjoiUC0yNTYiLCJ4IjoiVENBRVIxOVp2dTNPSEY0ajRXNHZmU1ZvSElQMUlMaWxEbHM3dkNlR2VtYyIsInkiOiJaeGppV1diWk1RR0hWV0tWUTRoYlNJaXJzVmZ1ZWNDRTZ0NGpUOUYySFpRIn19LCJhbGciOiJFUzI1NiJ9.OVSoCqHZLgAPaYK27gJx6J1ejwskP62xIHryqc1ZJYOR8yZdicSF4KXBk5qgocWZdiqEsri5Q3sW69xIfbmXSA~WyJseVMxN1ZzenNGb3doaFBnY3VuOTFRIiwgImV4cCIsIDE1NDE0OTQ3MjRd~WyJaRmNwSWxTNlJ5eWV2U3JTeFdJbDZRIiwgImdpdmVuX25hbWUiLCAiSm9obiJd~WyJVSHVVVUNlOWZzNUdody1mZ0JJWi13IiwgImZhbWlseV9uYW1lIiwgIkRvZSJd~WyJ3ZnR5YkpzYktzVWJDay1XaWpaQ3RRIiwgImVtYWlsIiwgInRlc3RAZXhhbXBsZS5jb20iXQ\"}";
            SetupHttpClientSequence(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(credentialResponse)
                });

            var expectedCredentialResponse = JsonConvert.DeserializeObject<OidCredentialResponse>(credentialResponse);

            var mockTokenResponse = new TokenResponse
            {
                AccessToken = "sampleAccessToken",
                TokenType = "bearer",
                ExpiresIn = 3600,
                CNonce = "sampleCNonce",
                CNonceExpiresIn = 3600
            };

            // Act
            var actualCredentialResponse = await _oid4VciClientService.RequestCredentialAsync(
                "https://issuer.io",
                "sampleClientNonce",
                "SimpleCredentialType",
                mockTokenResponse
            );

            // Assert
            actualCredentialResponse.Item1.Should().BeEquivalentTo(expectedCredentialResponse);
            actualCredentialResponse.Item2.Should().BeEquivalentTo(keyId);
        }

        [Fact]
        public async Task CanRequestTokenAsync()
        {
            // Arrange
            const string tokenResponse = "{\"access_token\":\"eyJhbGciOiJSUzI1NiIsInR5cCI6Ikp..sHQ\",\"token_type\":\"bearer\",\"expires_in\": 86400,\"c_nonce\": \"tZignsnFbp\",\"c_nonce_expires_in\":86400}";

            const string authServerMetadata =
                "{\"issuer\":\"https://issuer.io\",\"token_endpoint\":\"https://issuer.io/token\",\"token_endpoint_auth_methods_supported\":[\"urn:ietf:params:oauth:client-assertion-type:verifiable-presentation\"],\"response_types_supported\":[\"urn:ietf:params:oauth:grant-type:pre-authorized_code\"]}\n";

            var expectedTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

            SetupHttpClientSequence(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(authServerMetadata)
                },
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse)
                });

            // Act
            var actualTokenResponse =
                await _oid4VciClientService.RequestTokenAsync(_oidIssuerMetadata, "samplePreAuthorizedCode");

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

            _oid4VciClientService = new DefaultOid4VciClientService(_httpClientFactoryMock.Object, _keyStoreMock.Object);
        }
    }
}
