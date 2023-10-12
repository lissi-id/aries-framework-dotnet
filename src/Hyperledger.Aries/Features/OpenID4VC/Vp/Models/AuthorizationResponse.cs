namespace Hyperledger.Aries.Features.OpenId4Vc.Vp.Models
{
    /// <summary>
    ///  Represents an OpenID4VP Authorization Response.
    /// </summary>
    public class AuthorizationResponse
    {
        /// <summary>
        ///     Gets or sets the VP Token.
        /// </summary>
        public string VpToken { get; set; } = null!;
        
        /// <summary>
        ///   Gets or sets the Presentation Submission.
        /// </summary>
        public string PresentationSubmission { get; set; } = null!;
    }
}
