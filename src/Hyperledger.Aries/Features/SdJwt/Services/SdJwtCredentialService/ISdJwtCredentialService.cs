using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtCredentialService
{
    /// <summary>
    ///     Provides methods for handling SD-JWT credentials.
    /// </summary>
    public interface ISdJwtCredentialService
    {
        /// <summary>
        ///     Retrieves a specific SD-JWT record by its ID.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="credentialId">The ID of the SD-JWT credential record to retrieve.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the <see cref="SdJwtRecord" />
        ///     associated with the given ID.
        /// </returns>
        Task<SdJwtRecord> GetAsync(IAgentContext context, string credentialId);

        /// <summary>
        ///     Lists SD-JWT records based on specified criteria.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="query">The search query to filter SD-JWT records. Default is null, meaning no filter.</param>
        /// <param name="count">The maximum number of records to retrieve. Default is 100.</param>
        /// <param name="skip">The number of records to skip. Default is 0.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains a list of <see cref="SdJwtRecord" />
        ///     that match the criteria.
        /// </returns>
        Task<List<SdJwtRecord>> ListAsync(IAgentContext context, ISearchQuery query = null, int count = 100,
            int skip = 0);

        /// <summary>
        ///     Stores a new SD-JWT record.
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
