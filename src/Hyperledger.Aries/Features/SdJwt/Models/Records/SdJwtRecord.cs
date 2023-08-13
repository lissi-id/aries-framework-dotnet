#nullable enable

using System.Collections.Generic;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Models.Records
{
    /// <summary>
    ///     A record that represents a Selective Disclosure JSON Web Token (SD-JWT) Credential with additional properties.
    ///     Inherits from base class RecordBase.
    /// </summary>
    public class SdJwtRecord : RecordBase
    {
        /// <summary>
        ///     Gets or sets the attributes that should be displayed.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> DisplayedAttributes { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the claims made.
        /// </summary>
        public Dictionary<string, string> Claims { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the combined issuance.
        /// </summary>
        public string CombinedIssuance { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the identifier for the issuer.
        /// </summary>
        public string IssuerId { get; set; } = null!;

        /// <inheritdoc />
        public override string TypeName => "AF.SdJwtRecord";

        /// <summary>
        ///     Gets or sets the background color for the display.
        /// </summary>
        public string? BackgroundColor { get; set; }

        /// <summary>
        ///     Gets or sets the name of the issuer.
        /// </summary>
        public string? IssuerName { get; set; }

        /// <summary>
        ///     Gets or sets the logo to be displayed.
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        ///     Gets or sets the text color for the display.
        /// </summary>
        public string? TextColor { get; set; }
    }
}
