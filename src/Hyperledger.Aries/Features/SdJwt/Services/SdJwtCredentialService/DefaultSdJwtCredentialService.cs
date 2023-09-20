using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Features.OpenID4VP.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;
using SD_JWT.Abstractions;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtCredentialService
{
    /// <inheritdoc />
    public class DefaultSdJwtCredentialService : ISdJwtCredentialService
    {
        /// <summary>
        ///     The service responsible for holder operations.
        /// </summary>
        protected readonly IHolder Holder;

        /// <summary>
        ///     The service responsible for wallet record operations.
        /// </summary>
        protected readonly IWalletRecordService RecordService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultSdJwtCredentialService" /> class.
        /// </summary>
        /// <param name="recordService">The service responsible for wallet record operations.</param>
        /// <param name="holder">The service responsible for holder operations.</param>
        public DefaultSdJwtCredentialService(
            IHolder holder,
            IWalletRecordService recordService)
        {
            Holder = holder;
            RecordService = recordService;
        }

        /// <inheritdoc />
        public virtual async Task<SdJwtRecord> GetAsync(IAgentContext context, string credentialId)
        {
            var record = await RecordService.GetAsync<SdJwtRecord>(context.Wallet, credentialId);
            if (record == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "SD-JWT Credential record not found");

            return record;
        }

        /// <inheritdoc />
        public virtual Task<List<SdJwtRecord>> ListAsync(IAgentContext context, ISearchQuery query = null,
            int count = 100,
            int skip = 0)
        {
            return RecordService.SearchAsync<SdJwtRecord>(context.Wallet, query, null, count, skip);
        }

        /// <inheritdoc />
        public virtual async Task<string> StoreAsync(IAgentContext context, string combinedIssuance,
            string keyId, OidIssuerMetadata issuerMetadata)
        {
            var sdJwtDoc = Holder.ReceiveCredential(combinedIssuance);
            var record = SdJwtRecord.FromSdJwtDoc(sdJwtDoc);
            record.Id = Guid.NewGuid().ToString();

            record.SetDisplayFromIssuerMetadata(issuerMetadata);
            record.KeyId = keyId;

            await RecordService.AddAsync(context.Wallet, record);

            return record.Id;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(IAgentContext context, string recordId)
        {
            return await RecordService.DeleteAsync<SdJwtRecord>(context.Wallet, recordId);
        }

        public Task<CredentialCandidates[]> GetCredentialCandidates(InputDescriptor[] inputDescriptors)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateSdJwtPresentationFormat(InputDescriptor inputDescriptor, string credentialId)
        {
            throw new NotImplementedException();
        }
    }
}
