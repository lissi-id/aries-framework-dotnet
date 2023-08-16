using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;

/// <summary>
///     Represents the display attributes associated with a specific credential attribute in a credential subject.
/// </summary>
public class CredentialAttributeDisplays
{
    /// <summary>
    ///     Gets or sets the list of display properties associated with a specific credential attribute.
    /// </summary>
    /// <value>
    ///     The list of display properties. Each display property provides information on how the credential attribute should
    ///     be displayed.
    /// </value>
    [JsonProperty("display")]
    public List<Display> Display { get; set; }
}
