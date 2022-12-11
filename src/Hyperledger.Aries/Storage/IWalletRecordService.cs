﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Storage
{
    /// <summary>
    /// Wallet record service.
    /// </summary>
    public interface IWalletRecordService
    {
        /// <summary>
        /// Adds the record async.
        /// </summary>
        /// <returns>The record async.</returns>
        /// <param name="wallet">Wallet.</param>
        /// <param name="record">Record.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task AddAsync<T>(Wallet wallet, T record) where T : RecordBase, new();

        /// <summary>
        /// Searchs the records async.
        /// </summary>
        /// <returns>The records async.</returns>
        /// <param name="wallet">Wallet.</param>
        /// <param name="query">Query.</param>
        /// <param name="options">Options.</param>
        /// <param name="count">The number of items to return</param>
        /// <param name="skip">The number of items to skip</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<List<T>> SearchAsync<T>(
            Wallet wallet,
            ISearchQuery query = null,
            SearchOptions options = null,
            int count = 10,
            int skip = 0,
            [CallerMemberName] string callerName = null) where T : RecordBase, new();

        /// <summary>
        /// Updates the record async.
        /// </summary>
        /// <returns>The record async.</returns>
        /// <param name="wallet">Wallet.</param>
        /// <param name="record">Credential record.</param>
        Task UpdateAsync(Wallet wallet, RecordBase record);

        /// <summary>
        /// Gets the record async.
        /// </summary>
        /// <returns>The record async.</returns>
        /// <param name="wallet">Wallet.</param>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<T> GetAsync<T>(Wallet wallet, string id, [CallerMemberName] string callerName = null) where T : RecordBase, new();

        /// <summary>
        /// Deletes the record async.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <param name="wallet">Wallet.</param>
        /// <param name="id">Record Identifier.</param>
        /// <returns>Boolean status indicating if the removal succeed</returns>
        Task<bool> DeleteAsync<T>(Wallet wallet, string id) where T : RecordBase, new();
    }
}
