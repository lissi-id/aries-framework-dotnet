using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Services;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Storage;
using Moq;
using Moq.Protected;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.OpenId4Vc.Vp.Services
{
    public class Oid4VpClientServiceTests
    {
        private const string AuthRequest =
            "openid4vp://?client_id=https%3A%2F%2Fnc-sd-jwt.lambda.d3f.me%2Findex.php%2Fapps%2Fssi_login%2Foidc%2Fcallback&request_uri=https%3A%2F%2Fnc-sd-jwt.lambda.d3f.me%2Findex.php%2Fapps%2Fssi_login%2Foidc%2Frequestobject%2F4ba20ad0cb08545830aa549ab4305c03";

        private const string RequestUriResponse =
            "eyJhbGciOiJub25lIn0.eyJyZXNwb25zZV90eXBlIjoidnBfdG9rZW4iLCJjbGllbnRfaWQiOiJodHRwczpcL1wvbmMtc2Qtand0LmxhbWJkYS5kM2YubWVcL2luZGV4LnBocFwvYXBwc1wvc3NpX2xvZ2luXC9vaWRjXC9jYWxsYmFjayIsInJlZGlyZWN0X3VyaSI6Imh0dHBzOlwvXC9uYy1zZC1qd3QubGFtYmRhLmQzZi5tZVwvaW5kZXgucGhwXC9hcHBzXC9zc2lfbG9naW5cL29pZGNcL2NhbGxiYWNrIiwibm9uY2UiOiI4NzU1NDc4NDI2MDI4MDI4MDQ0MjA5MjE4NDE3MTI3NDEzMjQ1OCIsInByZXNlbnRhdGlvbl9kZWZpbml0aW9uIjp7ImlkIjoiNGRkMWMyNmEtMmY0Ni00M2FlLWE3MTEtNzA4ODhjOTNmYjRmIiwiaW5wdXRfZGVzY3JpcHRvcnMiOlt7ImlkIjoiTmV4dGNsb3VkQ3JlZGVudGlhbCIsImZvcm1hdCI6eyJ2YytzZC1qd3QiOnsicHJvb2ZfdHlwZSI6WyJKc29uV2ViU2lnbmF0dXJlMjAyMCJdfX0sImNvbnN0cmFpbnRzIjp7ImxpbWl0X2Rpc2Nsb3N1cmUiOiJyZXF1aXJlZCIsImZpZWxkcyI6W3sicGF0aCI6WyIkLnR5cGUiXSwiZmlsdGVyIjp7InR5cGUiOiJzdHJpbmciLCJjb25zdCI6IlZlcmlmaWVkRU1haWwifX0seyJwYXRoIjpbIiQuY3JlZGVudGlhbFN1YmplY3QuZW1haWwiXX1dfX1dfX0.";
        
        private Oid4VpClientService _oid4VpClientService;


        private readonly HttpResponseMessage _verifierRequestUriResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(RequestUriResponse)
        };

        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        private readonly Mock<IPexService> _pexServiceMock = new Mock<IPexService>();
        private readonly Mock<ISdJwtVcHolderService> _sdJwtVcHolderServiceMock = new Mock<ISdJwtVcHolderService>();

        [Fact]
        public async Task CanProcessAuthorizationRequest()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => _verifierRequestUriResponse);

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            
            _oid4VpClientService = new Oid4VpClientService(_pexServiceMock.Object, _sdJwtVcHolderServiceMock.Object, _httpClientFactoryMock.Object);
            //SetupHttpClientSequence(_issuerMetadataResponse);


            // Act
            var actualMetadata = await _oid4VpClientService.ProcessAuthorizationRequest(AuthRequest);

            // Assert
            actualMetadata.Should().BeEquivalentTo(RequestUriResponse);
        }
        
    }
}
