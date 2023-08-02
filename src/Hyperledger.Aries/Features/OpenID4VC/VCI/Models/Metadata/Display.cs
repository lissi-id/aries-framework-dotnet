#nullable enable

using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;

/// <summary>
///     Represents various visual representations or descriptions associated with a particular resource.
/// </summary>
public class Display
{
    /// <summary>
    ///     Initializes a new instance of the Display class.
    /// </summary>
    /// <param name="name">The name of the resource being represented.</param>
    public Display(string name)
    {
        Name = name;
    }
    
    /// <summary>
    ///     Gets or sets the logo associated with this representation.
    /// </summary>
    [JsonProperty("logo")]
    public Logo? Logo { get; set; }

    /// <summary>
    ///     Gets or sets the name of the resource being represented.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the background color for this representation.
    /// </summary>
    [JsonProperty("background_color")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    ///     Gets or sets the locale, which represents the specific culture or region, for which this representation is
    ///     intended.
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; set; }

    /// <summary>
    ///     Gets or sets the text color for this representation.
    /// </summary>
    [JsonProperty("text_color")]
    public string? TextColor { get; set; }
}
