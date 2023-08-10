using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenID4Common.Records;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation
{
    public interface IOpenId4VpClient
    {
        /// <summary>
        /// Processes the AuthenticationRequest url of an OpenId4VP request
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="url">The authentication request url.</param>
        /// <returns>The proof record wherein the request is stored.</returns>
        Task<string> ProcessAuthenticationRequestUrl(IAgentContext agentContext, string url);

        /// <summary>
        /// Generates the Authentication Response for OpenId4VP.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="authRecordId">The OpenId4VPRecord.</param>
        /// <param name="credRecordId">The credential record id.</param>
        /// <returns>Either null when the response method is direct_post otherwise it will return an Authentication
        /// Response Callback URL</returns>
        Task<string> GenerateAuthenticationResponse(IAgentContext agentContext, string authRecordId, string credRecordId, RequestedCredentials requestedCredentials);

        Task<OpenId4VpRecord> GetOpenId4VpRecordAsync(IAgentContext agentContext, string recordId);

        Task<List<OpenId4VpRecord>> ListOpenId4VpRecordAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100, int skip = 0);
    }
}
