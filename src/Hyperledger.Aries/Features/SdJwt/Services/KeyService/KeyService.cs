using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Services.KeyService
{
    /// <inheritdoc />
    public class KeyService : IKeyService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordService"></param>
        public KeyService(
            IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

        private readonly IWalletRecordService _recordService;

        /// <inheritdoc />
        public async Task<KeyRecord> GetAsync(IAgentContext context, string credentialId)
        {
            var record = await _recordService.GetAsync<KeyRecord>(context.Wallet, credentialId);
            if (record == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "SD-JWT Credential record not found");

            return record;
        }

        /// <inheritdoc />
        public Task<List<KeyRecord>> ListAsync(IAgentContext context, ISearchQuery query = null, int count = 100,
            int skip = 0)
        {
            return _recordService.SearchAsync<KeyRecord>(context.Wallet, query, null, count, skip);
        }

        /// <inheritdoc />
        public async Task<string> StoreFromKeyAliasAsync(IAgentContext context, string keyAlias)
        {
            var keyRecord = new KeyRecord
            {
                Id = Guid.NewGuid().ToString(),
                KeyAlias = keyAlias
            };

            await _recordService.AddAsync(context.Wallet, keyRecord);
            return keyRecord.Id;
        }
    }
}
