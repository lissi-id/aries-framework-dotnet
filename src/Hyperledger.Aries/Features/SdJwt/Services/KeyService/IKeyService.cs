using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Services.KeyService
{
    /// <summary>
    ///     Provides methods for managing and accessing key records.
    /// </summary>
    public interface IKeyService
    {
        /// <summary>
        ///     Retrieves a specific key record by its record ID.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="recordId">The ID of the key record to retrieve.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the <see cref="KeyRecord" />
        ///     associated with the given ID.
        /// </returns>
        Task<KeyRecord> GetAsync(IAgentContext context, string recordId);

        /// <summary>
        ///     Lists key records based on specified criteria.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="query">The search query to filter key records. Default is null, meaning no filter.</param>
        /// <param name="count">The maximum number of records to retrieve. Default is 100.</param>
        /// <param name="skip">The number of records to skip. Default is 0.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains a list of <see cref="KeyRecord" />
        ///     that match the criteria.
        /// </returns>
        Task<List<KeyRecord>> ListAsync(IAgentContext context, ISearchQuery query = null, int count = 100,
            int skip = 0);

        /// <summary>
        ///     Stores a new key record derived from a provided key alias.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="keyAlias">The key alias from which the key record is derived.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task result contains the ID of the stored key record.
        /// </returns>
        Task<string> StoreFromKeyAliasAsync(IAgentContext context, string keyAlias);
    }
}
