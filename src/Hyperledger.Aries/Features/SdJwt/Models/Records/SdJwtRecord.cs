#nullable enable

using System.Collections.Generic;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Models.Records;

/// <summary>
///     A record that represents a Selective Disclosure JSON Web Token (SD-JWT) Credential with additional properties.
///     Inherits from base class RecordBase.
/// </summary>
public class SdJwtRecord : RecordBase
{
    /// <summary>
    ///     Constructs a new instance of SdJwtRecord.
    /// </summary>
    /// <param name="issuerId">The unique identifier for the issuer.</param>
    /// <param name="displayedAttributes">
    ///     The attributes that should be displayed. These are represented as a dictionary where
    ///     each key represents a language, and the value is another dictionary containing the attribute name and value in that
    ///     language.
    /// </param>
    /// <param name="claims">
    ///     The claims made. These are represented as a dictionary where each key is the name of the value,
    ///     and the value is the actual value.
    /// </param>
    /// <param name="combinedIssuance">The combined issuance.</param>
    public SdJwtRecord(
        string issuerId,
        Dictionary<string, Dictionary<string, string>> displayedAttributes,
        Dictionary<string, string> claims,
        string combinedIssuance)
    {
        IssuerId = issuerId;
        DisplayedAttributes = displayedAttributes;
        Claims = claims;
        CombinedIssuance = combinedIssuance;
    }
    
    /// <summary>
    ///     Gets or sets the attributes that should be displayed.
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> DisplayedAttributes { get; set; }

    /// <summary>
    ///     Gets or sets the claims made.
    /// </summary>
    public Dictionary<string, string> Claims { get; set; }

    /// <summary>
    ///     Gets or sets the combined issuance.
    /// </summary>
    public string CombinedIssuance { get; set; }

    /// <summary>
    ///     Gets or sets the identifier for the issuer.
    /// </summary>
    public string IssuerId { get; set; }

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
