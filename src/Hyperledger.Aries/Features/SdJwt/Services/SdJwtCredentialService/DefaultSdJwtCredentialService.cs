using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.Metadata;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.KeyService;
using Hyperledger.Aries.Storage;
using SD_JWT.Abstractions;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtCredentialService
{
    /// <inheritdoc />
    public class DefaultSdJwtCredentialService : ISdJwtCredentialService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultSdJwtCredentialService" /> class.
        /// </summary>
        /// <param name="keyService">The service responsible for handling key records.</param>
        /// <param name="recordService">The service responsible for wallet record operations.</param>
        /// <param name="holder">The service responsible for holder operations.</param>
        public DefaultSdJwtCredentialService(
            IHolder holder,
            IKeyService keyService,
            IWalletRecordService recordService)
        {
            _holder = holder;
            _keyService = keyService;
            _recordService = recordService;
        }

        private readonly IHolder _holder;
        private readonly IKeyService _keyService;
        private readonly IWalletRecordService _recordService;

        /// <inheritdoc />
        public async Task<SdJwtRecord> GetAsync(IAgentContext context, string credentialId)
        {
            var record = await _recordService.GetAsync<SdJwtRecord>(context.Wallet, credentialId);
            if (record == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "SD-JWT Credential record not found");

            return record;
        }

        /// <inheritdoc />
        public Task<List<SdJwtRecord>> ListAsync(IAgentContext context, ISearchQuery query = null, int count = 100,
            int skip = 0)
        {
            return _recordService.SearchAsync<SdJwtRecord>(context.Wallet, query, null, count, skip);
        }

        /// <inheritdoc />
        public async Task<string> StoreAsync(IAgentContext context, string combinedIssuance,
            string keyId, OidIssuerMetadata issuerMetadata)
        {
            var sdJwtDoc = _holder.ReceiveCredential(combinedIssuance);
            var record = SdJwtRecord.FromSdJwtDoc(sdJwtDoc);
            record.Id = Guid.NewGuid().ToString();

            record.SetDisplayFromIssuerMetadata(issuerMetadata);

            var keyRecordId = await _keyService.StoreFromKeyAliasAsync(context, keyId);
            record.KeyRecordId = keyRecordId;

            await _recordService.AddAsync(context.Wallet, record);

            return record.Id;
        }
    }
}
