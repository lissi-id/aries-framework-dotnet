using JWT.Algorithms;

namespace Hyperledger.Aries.Features.OpenID4Common
{
    public interface IJwtSigningAlgorithm : IJwtAlgorithm
    {
        /// <summary>
        /// Returns the public key information as jwk
        /// </summary>
        /// <returns>The JsonWebKey.</returns>
        string GetJwk();
    }
}
