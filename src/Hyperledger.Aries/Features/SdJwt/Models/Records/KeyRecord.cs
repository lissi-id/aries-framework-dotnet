using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Models.Records
{
    /// <summary>
    /// Represents a record for storing a key alias related to a keystore.
    /// </summary>
    public class KeyRecord : RecordBase
    {
        /// <inheritdoc />
        public override string TypeName => "AF.KeyRecord";

        /// <summary>
        /// Gets or sets the unique identifier for the key.
        /// </summary>
        /// <value>
        /// The string representation of the key's unique identifier.
        /// </value>
        public string KeyAlias { get; set; }
    }
}
