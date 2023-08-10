namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Models.PresentationExchange
{
    public class SubmissionRequirement
    {
        public string Name { get; set; }
        public string Rule { get; set; }
        public int Count { get; set; }
        public string From { get; set; }
    }
}
