#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential.Attributes
{
    /// <summary>
    ///     Represents the display attributes associated with a specific credential attribute in a credential subject.
    /// </summary>
    public class OidCredentialSubjectAttribute
    {
        /// <summary>
        ///     Gets or sets the list of display properties associated with a specific credential attribute.
        /// </summary>
        /// <value>
        ///     The list of display properties. Each display property provides information on how the credential attribute should
        ///     be displayed.
        /// </value>
        [JsonProperty("display")]
        public List<OidCredentialAttributeDisplay>? Display { get; set; }

        /// <summary>
        ///     String value determining type of value of the claim. A non-exhaustive list of valid values defined by this
        ///     specification are string, number, and image media types such as image/jpeg.
        /// </summary>
        [JsonProperty("value_type")]
        public string? ValueType { get; set; }
    }
}