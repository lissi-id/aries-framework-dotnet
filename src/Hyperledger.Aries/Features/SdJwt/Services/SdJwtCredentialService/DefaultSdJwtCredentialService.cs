using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.SdJwt.Services.SdJwtCredentialService;

/// <inheritdoc />
public class DefaultSdJwtCredentialService : ISdJwtCredentialService
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultSdJwtCredentialService" /> class.
    /// </summary>
    /// <param name="recordService">The service responsible for wallet record operations.</param>
    public DefaultSdJwtCredentialService(
        IWalletRecordService recordService)
    {
        _recordService = recordService;
    }

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
    public async Task<string> StoreAsync(IAgentContext context, SdJwtRecord sdJwtRecord)
    {
        sdJwtRecord.Id = Guid.NewGuid().ToString();
        await _recordService.AddAsync(context.Wallet, sdJwtRecord);
        return sdJwtRecord.Id;
    }
}
