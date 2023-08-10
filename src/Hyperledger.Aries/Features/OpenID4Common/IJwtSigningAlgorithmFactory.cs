namespace Hyperledger.Aries.Features.OpenID4Common
{
    public interface IJwtSigningAlgorithmFactory
    {
        public IJwtSigningAlgorithm CreateAlgorithm(string keyAlias);
    }
}
