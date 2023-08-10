using System.Security.Cryptography;
using JWT.Algorithms;
using Microsoft.IdentityModel.Tokens;

namespace Hyperledger.Aries.Features.OpenID4Common
{
    public class MockJwtSigningAlgorithmFactory : IJwtSigningAlgorithmFactory
    {
        public IJwtSigningAlgorithm CreateAlgorithm(string keyAlias)
        {
            return new MockJwtSigningAlgorithm();
        }
    }

    public class MockJwtSigningAlgorithm : IJwtSigningAlgorithm
    {
        private const string Jwk = "{\n    \"kty\": \"EC\",\n    \"d\": \"Iw6qWZhQ04CtijWzp3q-vGrQfmOcKd1SqjlxMgqzvwA\",\n    \"use\": \"sig\",\n    \"crv\": \"P-256\",\n    \"kid\": \"ECSNPzYd7TefqsBXX6LvfskkZSU=\",\n    \"x\": \"xYrl9sGkLv6_K5xa8jQK1ixQ8FC9pKlkzq2e2Po4_VY\",\n    \"y\": \"a281dDn0k54m0wKl-SfqkXLESv4_G8wZEQWpvKmfO2w\",\n    \"alg\": \"ES256\"\n}";
        private readonly IJwtAlgorithm _jwtAlgorithm;

        public MockJwtSigningAlgorithm()
        {
            var jsonWebKey = new JsonWebKey(Jwk);
            var x = Base64UrlEncoder.DecodeBytes(jsonWebKey.X);
            var y = Base64UrlEncoder.DecodeBytes(jsonWebKey.Y);
            var d = Base64UrlEncoder.DecodeBytes(jsonWebKey.D);

            ECDsa ecdsa = ECDsa.Create(new ECParameters()
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = d,
                Q = new ECPoint {X = x, Y = y}
            })!;
            ECDsaSecurityKey key = new ECDsaSecurityKey(ecdsa);

            _jwtAlgorithm = new ES256Algorithm(key.ECDsa, key.ECDsa);
        }
        
        public byte[] Sign(byte[] key, byte[] bytesToSign)
        {
            return _jwtAlgorithm.Sign(key, bytesToSign);
        }

        public string Name => _jwtAlgorithm.Name;
        public HashAlgorithmName HashAlgorithmName => _jwtAlgorithm.HashAlgorithmName;
        public string GetJwk()
        {
            return Jwk;
        }
    }
}
