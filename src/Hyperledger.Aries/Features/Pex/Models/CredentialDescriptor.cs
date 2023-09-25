namespace Hyperledger.Aries.Features.Pex.Models
{
    /// <summary>
    /// The credential descriptor.
    /// </summary>
    public class CredentialDescriptor : Descriptor
    {
        /// <summary>
        /// This MUST be the id value of a credential.
        /// </summary>
        public string CredentialId { get; set; } = null!;
    }
}
