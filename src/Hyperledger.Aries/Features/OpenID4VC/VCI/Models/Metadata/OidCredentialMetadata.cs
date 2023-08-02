#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;

/// <summary>
///     Represents the metadata of a specific type of credential that a Credential Issuer can issue.
/// </summary>
public class OidCredentialMetadata
{
    /// <summary>
    ///     Initializes a new instance of the OidCredentialMetadata class.
    /// </summary>
    /// <param name="format">Identifier for the format of the credential.</param>
    /// <param name="type">Type of the credential.</param>
    /// <param name="credentialSubject">Dictionary representing the attributes of the credential in different languages.</param>
    public OidCredentialMetadata(
        string format,
        string type,
        Dictionary<string, Display> credentialSubject)
    {
        Format = format;
        Type = type;
        CredentialSubject = credentialSubject;
    }
    
    /// <summary>
    ///     Gets or sets the dictionary representing the attributes of the credential in different languages.
    /// </summary>
    [JsonProperty("credentialSubject")]
    public Dictionary<string, Display> CredentialSubject { get; set; }

    /// <summary>
    ///     Gets or sets a list of display properties of the supported credential for different languages.
    /// </summary>
    [JsonProperty("display")]
    public List<Display>? Display { get; set; }

    /// <summary>
    ///     Gets or sets a list of methods that identify how the Credential is bound to the identifier of the End-User who
    ///     possesses the Credential.
    /// </summary>
    [JsonProperty("cryptographic_binding_methods_supported")]
    public List<string>? CryptographicBindingMethodsSupported { get; set; }

    /// <summary>
    ///     Gets or sets a list of identifiers for the cryptographic suites that are supported.
    /// </summary>
    [JsonProperty("cryptographic_suites_supported")]
    public List<string>? CryptographicSuitesSupported { get; set; }

    /// <summary>
    ///     Gets or sets the identifier for the format of the credential.
    /// </summary>
    [JsonProperty("format")]
    public string Format { get; set; }

    /// <summary>
    ///     Gets or sets the type of the credential.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier for the respective credential.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }
}
