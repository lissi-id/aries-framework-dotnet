#nullable enable

using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential
{
    /// <summary>
    ///     Represents the visual representations for the credential.
    /// </summary>
    public class OidCredentialDisplay
    {
        /// <summary>
        ///     Gets or sets the logo associated with this Credential.
        /// </summary>
        [JsonProperty("logo")]
        public OidCredentialLogo? Logo { get; set; }

        /// <summary>
        ///     Gets or sets the name of the Credential.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the background color for the Credential.
        /// </summary>
        [JsonProperty("background_color")]
        public string? BackgroundColor { get; set; }

        /// <summary>
        ///     Gets or sets the locale, which represents the specific culture or region.
        /// </summary>
        [JsonProperty("locale")]
        public string? Locale { get; set; }

        /// <summary>
        ///     Gets or sets the text color for the Credential.
        /// </summary>
        [JsonProperty("text_color")]
        public string? TextColor { get; set; }
    }
}
