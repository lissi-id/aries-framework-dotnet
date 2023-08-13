#nullable enable

using System;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata
{
    /// <summary>
    ///     Represents a graphical identity or brand representation of a particular resource.
    /// </summary>
    public class Logo
    {
        /// <summary>
        ///     Gets or sets the alternate text that describes the logo image. This is typically used for accessibility purposes.
        /// </summary>
        [JsonProperty("alt_text")]
        public string? AltText { get; set; }

        /// <summary>
        ///     Gets or sets the URL of the logo image.
        /// </summary>
        [JsonProperty("url")]
        public Uri? Url { get; set; }
    }
}
