using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Services;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.Pex.Services;
using Hyperledger.Aries.Tests.Extensions;
using Moq;
using Moq.Protected;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.OpenId4Vc.Vp.Services
{
    public class Oid4VpHaipClientTests
    {
        private const string AuthRequestWithRequestUri =
            "haip://?client_id=https%3A%2F%2Fverifier.com%2Fsome%2Fcallback&request_uri=https%3A%2F%2Fverifier.com%2Fsome%2Frequestobject%2F4ba20ad0cb08545830aa549ab4305c03";

        private const string RequestUriResponse =
            "eyJhbGciOiJFUzI1NiJ9.eyJyZXNwb25zZV90eXBlIjoidnBfdG9rZW4iLCJyZXNwb25zZV9tb2RlIjoiZGlyZWN0X3Bvc3QiLCJjbGllbnRfaWQiOiJodHRwczovL3ZlcmlmaWVyLmNvbS9zb21lL2NhbGxiYWNrIiwicmVzcG9uc2VfdXJpIjoiaHR0cHM6Ly92ZXJpZmllci5jb20vc29tZS9jYWxsYmFjayIsIm5vbmNlIjoiODc1NTQ3ODQyNjAyODAyODA0NDIwOTIxODQxNzEyNzQxMzI0NTgiLCJwcmVzZW50YXRpb25fZGVmaW5pdGlvbiI6eyJpZCI6IjRkZDFjMjZhLTJmNDYtNDNhZS1hNzExLTcwODg4YzkzZmI0ZiIsImlucHV0X2Rlc2NyaXB0b3JzIjpbeyJpZCI6IlRlc3RDcmVkZW50aWFsIiwiZm9ybWF0Ijp7InZjK3NkLWp3dCI6eyJwcm9vZl90eXBlIjpbIkpzb25XZWJTaWduYXR1cmUyMDIwIl19fSwiY29uc3RyYWludHMiOnsibGltaXRfZGlzY2xvc3VyZSI6InJlcXVpcmVkIiwiZmllbGRzIjpbeyJwYXRoIjpbIiQudHlwZSJdLCJmaWx0ZXIiOnsidHlwZSI6InN0cmluZyIsImNvbnN0IjoiVmVyaWZpZWRFTWFpbCJ9fSx7InBhdGgiOlsiJC5jcmVkZW50aWFsU3ViamVjdC5lbWFpbCJdfV19fV19fQ.BMGJeS-BIj0zrKxmdhFBht4fQEkWazreubSWSgo260SCFWkp3gNXApQ4sq6WYI5923bgFzIS7Yq6NUiiQEdf9A";

        private Oid4VpHaipClient _oid4VpHaipClient;
        
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        [Theory]
        [InlineData(AuthRequestWithRequestUri, RequestUriResponse)]
        public async Task CanProcessAuthorizationRequest(string authorizationRequestUri, string httpResponse)
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(httpResponse)
            };
            
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => httpResponseMessage);

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            
            _oid4VpHaipClient = new Oid4VpHaipClient(_httpClientFactoryMock.Object, new PexService());
            
            var expectedAuthorizationRequest = GetExpectedAuthorizationRequest();

            // Act
            var authorizationRequest = await _oid4VpHaipClient.ProcessAuthorizationRequestAsync(
                    HaipAuthorizationRequestUri.FromUri(new Uri(authorizationRequestUri)));

            // Assert
            authorizationRequest.Should().BeEquivalentTo(expectedAuthorizationRequest);
        }

        private AuthorizationRequest GetExpectedAuthorizationRequest()
        {
            var format = new Format();
            format.PrivateSet(x => x.ProofTypes, new[] { "JsonWebSignature2020" });
            
            var filter = new Filter();
            filter.PrivateSet(x => x.Type, "string");
            filter.PrivateSet(x => x.Const, "VerifiedEMail");
            
            var fieldOne = new Field();
            fieldOne.PrivateSet(x => x.Filter, filter);
            fieldOne.PrivateSet(x => x.Path, new [] {"$.type"});
            
            var fieldTwo = new Field();
            fieldTwo.PrivateSet(x => x.Path, new [] {"$.credentialSubject.email"});
            
            var constraints = new Constraints();
            constraints.PrivateSet(x => x.LimitDisclosure, "required");
            constraints.PrivateSet(x => x.Fields, new [] {fieldOne, fieldTwo});
            
            var inputDescriptor = new InputDescriptor();
            inputDescriptor.PrivateSet(x => x.Id, "TestCredential");
            inputDescriptor.PrivateSet(x => x.Formats, new Dictionary<string, Format> { {"vc+sd-jwt", format }});
            inputDescriptor.PrivateSet(x => x.Constraints, constraints);
            
            var presentationDefinition = new PresentationDefinition();
            presentationDefinition.PrivateSet(x => x.Id, "4dd1c26a-2f46-43ae-a711-70888c93fb4f");
            presentationDefinition.PrivateSet(x => x.InputDescriptors, new[] { inputDescriptor });
            
            return new AuthorizationRequest()
            {
                ResponseType = "vp_token",
                ClientId = "https://verifier.com/some/callback",
                ResponseUri = "https://verifier.com/some/callback",
                Nonce = "87554784260280280442092184171274132458",
                ResponseMode = "direct_post",
                PresentationDefinition = presentationDefinition,
            };
        }
        
    }
}
