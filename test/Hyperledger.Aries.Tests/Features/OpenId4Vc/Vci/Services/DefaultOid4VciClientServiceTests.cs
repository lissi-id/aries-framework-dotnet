using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.OpenId4Vc.KeyStore.Services;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Authorization;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.CredentialResponse;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential.Attributes;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Services.Oid4VciClientService;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.OpenId4Vc.Vci.Services
{
    public class DefaultOid4VciClientServiceTests
    {
        private const string AuthServerMetadata =
            "{\"issuer\":\"https://issuer.io\",\"token_endpoint\":\"https://issuer.io/token\",\"token_endpoint_auth_methods_supported\":[\"urn:ietf:params:oauth:client-assertion-type:verifiable-presentation\"],\"response_types_supported\":[\"urn:ietf:params:oauth:grant-type:pre-authorized_code\"]}\n";

        private const string CredentialType = "VerifiedEmail";

        private const string IssuerMetadataResponseContent =
            "{\"credential_issuer\":\"https://issuer.io/\",\"credential_endpoint\":\"https://issuer.io/credential\",\"display\":[{\"name\":\"Aussteller\",\"locale\":\"de-DE\"},{\"name\":\"Issuer\",\"locale\":\"en-US\"}],\"credentials_supported\":[{\"format\":\"vc+sd-jwt\",\"id\":\"VerifiedEMail\",\"cryptographic_binding_methods_supported\":[\"jwk\"],\"cryptographic_suites_supported\":[\"ES256\"],\"type\":\"VerifiedEMail\",\"display\":[{\"name\":\"Verifizierte eMail-Adresse\",\"logo\":{\"url\":\"https://issuer.io/logo.png\",\"alternative_text\":\"Logo\"},\"text_color\":\"#FFFFFF\",\"background_color\":\"#12107c\",\"locale\":\"de-DE\"},{\"name\":\"Verified eMail address\",\"logo\":{\"url\":\"https://issuer.io/logo.png\",\"alternative_text\":\"Logo\"},\"text_color\":\"#FFFFFF\",\"background_color\":\"#12107c\",\"locale\":\"en-US\"}],\"credentialSubject\":{\"given_name\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"Vorname\"},{\"locale\":\"en-US\",\"name\":\"Given name\"}]},\"last_name\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"Nachname\"},{\"locale\":\"en-US\",\"name\":\"Surname\"}]},\"email\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"E-Mail Adresse\"},{\"locale\":\"en-US\",\"name\":\"e-Mail address\"}]}}},{\"format\":\"vc+sd-jwt\",\"id\":\"AttestedVerifiedEMail\",\"cryptographic_binding_methods_supported\":[\"jwk\"],\"cryptographic_suites_supported\":[\"ES256\"],\"type\":\"AttestedVerifiedEMail\",\"display\":[{\"name\":\"Verifizierte eMail-Adresse\",\"logo\":{\"url\":\"https://issuer.io/logo.png\",\"alternative_text\":\"Logo\"},\"text_color\":\"#FFFFFF\",\"background_color\":\"#12107c\",\"locale\":\"de-DE\"},{\"name\":\"Verified eMail address\",\"logo\":{\"url\":\"https://issuer.io/logo.png\",\"alternative_text\":\"Logo\"},\"text_color\":\"#FFFFFF\",\"background_color\":\"#12107c\",\"locale\":\"en-US\"}],\"credentialSubject\":{\"given_name\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"Vorname\"},{\"locale\":\"en-US\",\"name\":\"Given name\"}]},\"last_name\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"Nachname\"},{\"locale\":\"en-US\",\"name\":\"Surname\"}]},\"email\":{\"display\":[{\"locale\":\"de-DE\",\"name\":\"E-Mail Adresse\"},{\"locale\":\"en-US\",\"name\":\"e-Mail address\"}]}}}]}";

        private const string PreAuthorizedCode = "1234";

        private const string TokenResponse =
            "{\"access_token\":\"eyJhbGciOiJSUzI1NiIsInR5cCI6Ikp..sHQ\",\"token_type\":\"bearer\",\"expires_in\": 86400,\"c_nonce\": \"tZignsnFbp\",\"c_nonce_expires_in\":86400}";

        private DefaultOid4VciClientService _oid4VciClientService;

        private readonly HttpResponseMessage _authServerMetadataResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(AuthServerMetadata)
        };

        private readonly HttpResponseMessage _issuerMetadataResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(IssuerMetadataResponseContent)
        };

        private readonly HttpResponseMessage _tokenResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(TokenResponse)
        };

        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        private readonly Mock<IKeyStore> _keyStoreMock = new Mock<IKeyStore>();

        private readonly OidIssuerMetadata _oidIssuerMetadata = new OidIssuerMetadata
        {
            CredentialIssuer = "https://issuer.io",
            CredentialEndpoint = "https://issuer.io/credential",
            CredentialsSupported = new List<OidCredentialMetadata>
            {
                new OidCredentialMetadata
                {
                    Format = "vc+sdjwt",
                    Type = CredentialType,
                    CredentialSubject = new Dictionary<string, OidCredentialSubjectAttribute>()
                }
            }
        };

        [Fact]
        public async Task AuthServerUriIsBuiltCorrectly()
        {
            // Arrange
            const string authServerUri = "https://authserver.io";
            _oidIssuerMetadata.AuthorizationServer = authServerUri;

            SetupHttpClientSequence(_authServerMetadataResponse, _tokenResponse);

            var expectedUri = new Uri(authServerUri);

            // Act
            await _oid4VciClientService.RequestTokenAsync(_oidIssuerMetadata, PreAuthorizedCode);

            // Assert
            _httpMessageHandlerMock.Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData("https://issuer.io", "https://issuer.io/.well-known/oauth-authorization-server")]
        [InlineData("https://issuer.io/issuer1", "https://issuer.io/.well-known/oauth-authorization-server/issuer1")]
        public async Task AuthServerUriIsBuiltFromCredentialIssuerCorrectly(string issuer, string expectedUriString)
        {
            // Arrange
            _oidIssuerMetadata.CredentialIssuer = issuer;

            SetupHttpClientSequence(_authServerMetadataResponse, _tokenResponse);

            var expectedUri = new Uri(expectedUriString);

            // Act
            await _oid4VciClientService.RequestTokenAsync(_oidIssuerMetadata, PreAuthorizedCode);

            // Assert
            _httpMessageHandlerMock.Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CanGetIssuerMetadataAsync()
        {
            // Arrange
            SetupHttpClientSequence(_issuerMetadataResponse);

            var expectedMetadata = JsonConvert.DeserializeObject<OidIssuerMetadata>(IssuerMetadataResponseContent);

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

            _keyStoreMock.Setup(j => j.GenerateKey(It.IsAny<string>()))
                .ReturnsAsync(keyId);
            _keyStoreMock.Setup(j =>
                    j.GenerateProofOfPossessionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(jwtMock);

            const string credentialResponse =
                "{\"format\":\"vc+sd-jwt\",\"credential\":\"eyJhbGciOiAiRVMyNTYifQ.eyJfc2QiOlsiT0dfT2lCMk5ZS0JzTVhIOFVVb2luREhUT1h5VER1Z3JPdE94RFI2NF9ZcyIsIlQzbHRYQUFtODNJTXRUYkRTb1J2d1g2Tk10em1scV9ZWG9Vd1EwZDY0NEUiXSwiaXNzIjoiaHR0cHM6Ly9pc3N1ZXIuaW8vIiwiaWF0IjoxNTE2MjM5MDIyLCJ0eXBlIjoiVmVyaWZpZWRFbWFpbCIsImV4cCI6MTUxNjI0NzAyMiwiX3NkX2FsZyI6InNoYS0yNTYiLCJjbmYiOnsiandrIjp7Imt0eSI6IkVDIiwiY3J2IjoiUC0yNTYiLCJ4IjoiVENBRVIxOVp2dTNPSEY0ajRXNHZmU1ZvSElQMUlMaWxEbHM3dkNlR2VtYyIsInkiOiJaeGppV1diWk1RR0hWV0tWUTRoYlNJaXJzVmZ1ZWNDRTZ0NGpUOUYySFpRIn19LCJhbGciOiJFUzI1NiJ9.OVSoCqHZLgAPaYK27gJx6J1ejwskP62xIHryqc1ZJYOR8yZdicSF4KXBk5qgocWZdiqEsri5Q3sW69xIfbmXSA~WyJseVMxN1ZzenNGb3doaFBnY3VuOTFRIiwgImV4cCIsIDE1NDE0OTQ3MjRd~WyJaRmNwSWxTNlJ5eWV2U3JTeFdJbDZRIiwgImdpdmVuX25hbWUiLCAiSm9obiJd~WyJVSHVVVUNlOWZzNUdody1mZ0JJWi13IiwgImZhbWlseV9uYW1lIiwgIkRvZSJd~WyJ3ZnR5YkpzYktzVWJDay1XaWpaQ3RRIiwgImVtYWlsIiwgInRlc3RAZXhhbXBsZS5jb20iXQ\"}";
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
                _oidIssuerMetadata.CredentialIssuer,
                mockTokenResponse.CNonce,
                CredentialType,
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
            SetupHttpClientSequence(
                _authServerMetadataResponse,
                _tokenResponse);

            var expectedTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(TokenResponse);

            // Act
            var actualTokenResponse =
                await _oid4VciClientService.RequestTokenAsync(_oidIssuerMetadata, PreAuthorizedCode);

            // Assert
            actualTokenResponse.Should().BeEquivalentTo(expectedTokenResponse);
        }

        [Theory]
        [InlineData("https://issuer.io", "https://issuer.io/.well-known/openid-credential-issuer")]
        [InlineData("https://issuer.io/issuer1", "https://issuer.io/issuer1/.well-known/openid-credential-issuer")]
        public async Task CredentialIssuerUriIsBuiltCorrectly(string inputUri, string expectedUriString)
        {
            // Arrange
            SetupHttpClientSequence(_issuerMetadataResponse);

            // Act
            var expectedUri = new Uri(expectedUriString);
            await _oid4VciClientService.FetchIssuerMetadataAsync(new Uri(inputUri));

            // Assert
            _httpMessageHandlerMock.Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                    ItExpr.IsAny<CancellationToken>());
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

            _oid4VciClientService =
                new DefaultOid4VciClientService(_httpClientFactoryMock.Object, _keyStoreMock.Object);
        }
    }
}
