using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OpenID4VC.JWT.Services;

/// <summary>
///     Represents a factory for creating JSON Web Tokens (JWT) using a hardware key.
///     This interface is intended to be implemented outside of the framework on the device side,
///     due to the dependence on hardware-specific key generation or retrieval mechanisms.
/// </summary>
public interface IJwtFactory
{
    /// <summary>
    ///     Asynchronously creates a JWT based on the provided audience and nonce, utilizing a hardware key.
    /// </summary>
    /// <param name="audience">The intended recipient of the JWT. Typically represents the entity that will verify the JWT.</param>
    /// <param name="nonce">
    ///     A unique token, typically used to prevent replay attacks by ensuring that the JWT is only used
    ///     once.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous operation. When evaluated, the task's result
    ///     contains a JWT string.
    /// </returns>
    Task<string> CreateJwtFromHardwareKeyAsync(string audience, string nonce);
}
