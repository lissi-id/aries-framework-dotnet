using System.Collections.Generic;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Features.DidExchange
{
    /// <summary>
    /// Extensions for interacting with DID docs.
    /// </summary>
    public static class DidDocExtensions
    {
        /// <summary>
        /// Default key type.
        /// </summary>
        public const string DefaultKeyType = "Ed25519VerificationKey2018";
        
        /// <summary>
        /// Constructs my DID doc in a pairwise relationship from a connection record and the agents provisioning record.
        /// </summary>
        /// <param name="connection">Connection record.</param>
        /// <param name="provisioningRecord">Provisioning record.</param>
        /// <returns>DID Doc</returns>
        public static DidDoc MyDidDoc(this ConnectionRecord connection, ProvisioningRecord provisioningRecord)
        {
            var theirKey = connection.TheirVk ?? connection.GetTag("InvitationKey");
            var useDidKey = string.IsNullOrWhiteSpace(theirKey) == false && DidUtils.IsDidKey(theirKey);
            var myDidKey = connection.MyVk != null && useDidKey ? DidUtils.ConvertVerkeyToDidKey(connection.MyVk) : null;  
            
            var id = myDidKey ?? $"did:example:{connection.MyDid}";
            var doc = new DidDoc
            {
                Id = id,
                Keys = new List<DidDocKey>
                {
                    new DidDocKey
                    {
                        Id = $"{id}#keys-1",
                        Type = DefaultKeyType,
                        Controller = id,
                        PublicKeyBase58 = connection.MyVk
                    }
                }
            };

            if (!string.IsNullOrEmpty(provisioningRecord.Endpoint.Uri))
            {
                var recipientKey = myDidKey ?? connection.MyVk;
                doc.Services = new List<IDidDocServiceEndpoint>
                {
                    new IndyAgentDidDocService
                    {
                        Id = $"{id};indy",
                        ServiceEndpoint = provisioningRecord.Endpoint.Uri,
                        RecipientKeys = recipientKey != null ? new []{ recipientKey } : new string[0],
                        RoutingKeys = provisioningRecord.Endpoint?.Verkey != null ? provisioningRecord.Endpoint.Verkey : new string[0]
                    }
                };
            }

            return doc;
        }

        /// <summary>
        /// Constructs their DID doc in a pairwise relationship from a connection record.
        /// </summary>
        /// <param name="connection">Connectio record.</param>
        /// <returns>DID Doc</returns>
        public static DidDoc TheirDidDoc(this ConnectionRecord connection)
        {
            var theirKey = connection.TheirVk ?? connection.GetTag("InvitationKey");
            var isDidKey = string.IsNullOrWhiteSpace(theirKey) == false && DidUtils.IsDidKey(theirKey);

            var id = isDidKey ? theirKey : $"did:example:{connection.TheirDid}";
            var verKey = isDidKey ? DidUtils.ConvertDidKeyToVerkey(connection.TheirVk) : connection.TheirVk;
            return new DidDoc
            {
                Id = id,
                Keys = new List<DidDocKey>
                {
                    new DidDocKey
                    {
                        Id = $"{id}#keys-1",
                        Type = DefaultKeyType,
                        Controller = id,
                        PublicKeyBase58 = verKey
                    }
                },
                Services = new List<IDidDocServiceEndpoint>
                {
                    new IndyAgentDidDocService
                    {
                        Id = $"{id};indy",
                        ServiceEndpoint = connection.Endpoint.Uri,
                        RecipientKeys = verKey != null ? new[] { verKey } : new string[0],
                        RoutingKeys = connection.Endpoint?.Verkey != null ? connection.Endpoint.Verkey : new string[0]
                    }
                }
            };
        }
    }
}
