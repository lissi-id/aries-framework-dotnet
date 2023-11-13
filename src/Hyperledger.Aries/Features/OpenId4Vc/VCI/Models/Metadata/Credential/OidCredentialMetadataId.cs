namespace Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential
{
    public struct OidCredentialMetadataId
    {
        public string Value { get; private set; }
        
        public OidCredentialMetadataId(string value)
        {
            Value = value;
        }
    }
}
