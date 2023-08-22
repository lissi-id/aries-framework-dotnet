using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OpenId4Vc.KeyStore.Services
{
    /// <summary>
    ///     Represents a store for managing keys.
    ///     This interface is intended to be implemented outside of the framework on the device side,
    ///     allowing flexibility in key generation or retrieval mechanisms.
    /// </summary>
    public interface IKeyStore
    {
        /// <summary>
        ///     Asynchronously creates a proof of possession based on the provided audience and nonce.
        /// </summary>
        /// <param name="audience">The intended recipient of the proof. Typically represents the entity that will verify it.</param>
        /// <param name="nonce">
        ///     A unique token, typically used to prevent replay attacks by ensuring that the proof is only used
        ///     once.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}" /> representing the asynchronous operation. When evaluated, the task's result
        ///     contains two strings, the first being the proof and the second being the key ID.
        /// </returns>
        Task<(string Proof, string KeyId)> CreateProofOfPossessionAsync(string audience, string nonce);
    }
}
