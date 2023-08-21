using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtCredentialService
{
    /// <summary>
    ///     Provides methods for handling JWT credentials.
    /// </summary>
    public interface ISdJwtCredentialService
    {
        /// <summary>
        ///     Retrieves a specific JWT record by its ID.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="credentialId">The ID of the JWT credential record to retrieve.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the <see cref="SdJwtRecord" />
        ///     associated with the given ID.
        /// </returns>
        Task<SdJwtRecord> GetAsync(IAgentContext context, string credentialId);

        /// <summary>
        ///     Lists JWT records based on specified criteria.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="query">The search query to filter JWT records. Default is null, meaning no filter.</param>
        /// <param name="count">The maximum number of records to retrieve. Default is 100.</param>
        /// <param name="skip">The number of records to skip. Default is 0.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains a list of <see cref="SdJwtRecord" />
        ///     that match the criteria.
        /// </returns>
        Task<List<SdJwtRecord>> ListAsync(IAgentContext context, ISearchQuery query = null, int count = 100,
            int skip = 0);

        /// <summary>
        ///     Stores a new JWT record.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="combinedIssuance">The combined issuance.</param>
        /// <param name="keyId">The key id.</param>
        /// <param name="issuerMetadata">The issuer metadata.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the ID of the stored JWT record.</returns>
        Task<string> StoreAsync(IAgentContext context, string combinedIssuance, string keyId,
            OidIssuerMetadata issuerMetadata);
    }
}
