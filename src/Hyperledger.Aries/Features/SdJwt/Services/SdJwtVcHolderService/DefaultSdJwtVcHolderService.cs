using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SD_JWT.Abstractions;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService
{
    /// <inheritdoc />
    public class DefaultSdJwtVcHolderService : ISdJwtVcHolderService
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
        ///     Initializes a new instance of the <see cref="DefaultSdJwtVcHolderService" /> class.
        /// </summary>
        /// <param name="recordService">The service responsible for wallet record operations.</param>
        /// <param name="holder">The service responsible for holder operations.</param>
        public DefaultSdJwtVcHolderService(
            IHolder holder,
            IWalletRecordService recordService)
        {
            Holder = holder;
            RecordService = recordService;
        }

        public Task<string> CreateSdJwtPresentationFormatAsync(InputDescriptor inputDescriptors, string credentialId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(IAgentContext context, string recordId)
        {
            return await RecordService.DeleteAsync<SdJwtRecord>(context.Wallet, recordId);
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
        public virtual Task<CredentialCandidates[]> GetCredentialCandidates(SdJwtRecord[] credentials,
            InputDescriptor[] inputDescriptors)
        {
            var result = new List<CredentialCandidates>();

            foreach (var inputDescriptor in inputDescriptors)
            {
                if (!inputDescriptor.Formats.Keys.Contains("vc+sd-jwt"))
                {
                    continue;
                }

                var credentialCandidates = new CredentialCandidates();

                if (inputDescriptor.Constraints.Fields == null)
                {
                    credentialCandidates.InputDescriptorId = inputDescriptor.Id;
                    credentialCandidates.Credentials.AddRange(credentials);
                    result.Add(credentialCandidates);
                    continue;
                }

                var matchingCredentials =
                    FindMatchingCredentialsForFields(credentials, inputDescriptor.Constraints.Fields);
                if (matchingCredentials.Length == 0)
                {
                    continue;
                }

                credentialCandidates.InputDescriptorId = inputDescriptor.Id;
                credentialCandidates.Credentials.AddRange(matchingCredentials);

                if (string.Equals(inputDescriptor.Constraints.LimitDisclosure, "required"))
                {
                    credentialCandidates.LimitDisclosuresRequired = true; 
                }
                
                if (inputDescriptor.Group != null)
                {
                    credentialCandidates.Group = inputDescriptor.Group;
                }
                
                result.Add(credentialCandidates);
            }

            return Task.FromResult(result.ToArray());
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

        private static SdJwtRecord[] FindMatchingCredentialsForFields(
            SdJwtRecord[] records, Field[] fields)
        {
            return (from sdJwtRecord in records
                let claimsJson = JsonConvert.SerializeObject(sdJwtRecord.Claims)
                let claimsJObject = JObject.Parse(claimsJson)
                let isFound =
                    (from field in fields
                        let candidate = claimsJObject.SelectToken(field.Path[0])
                        where candidate != null && (field.Filter == null ||
                                                    string.Equals(field.Filter.Const, candidate.ToString()))
                        select field).Count() == fields.Length
                where isFound
                select sdJwtRecord).ToArray();
        }
    }
}
