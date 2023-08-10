using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OpenID4VC.JWT;

public interface IJwtFactory
{
    Task<string> CreateProofOfPossessionJwtAsync(string audience, string nonce);
}
