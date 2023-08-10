using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenID4Common.Records
{
    public class OpenId4VpRecord : RecordBase
    {
        public string AuthenticationRequest { get; set; }
        public string ResponseType { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string PresentationDefinition { get; set; }
        public string Nonce { get; set; }
        public string Registration { get; set; }
        public string ResponseMode { get; set; }
        public override string TypeName => "OpenId.VpRecord";
    }
}
