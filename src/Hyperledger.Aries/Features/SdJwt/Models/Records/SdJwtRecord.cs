#nullable enable

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using Hyperledger.Aries.Features.OpenID4VC.VCI.Models.CredentialOffer;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Credential.Attributes;
using Hyperledger.Aries.Features.OpenId4Vc.Vci.Models.Metadata.Issuer;
using Hyperledger.Aries.Storage;
using SD_JWT.Models;

namespace Hyperledger.Aries.Features.SdJwt.Models.Records
{
    /// <summary>
    ///     A record that represents a Selective Disclosure JSON Web Token (SD-JWT) Credential with additional properties.
    ///     Inherits from base class RecordBase.
    /// </summary>
    public class SdJwtRecord : RecordBase
    {
        /// <summary>
        ///     Gets or sets the attributes that should be displayed.
        /// </summary>
        public Dictionary<string, OidCredentialSubjectAttribute>? DisplayedAttributes { get; set; }

        /// <summary>
        ///     Gets or sets the claims made.
        /// </summary>
        public Dictionary<string, string> Claims { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the name of the issuer in different languages.
        /// </summary>
        public Dictionary<string, string>? IssuerName { get; set; }

        /// <summary>
        /// </summary>
        public List<OidCredentialDisplay>? Display { get; set; }

        /// <summary>
        ///     Gets or sets the combined issuance.
        /// </summary>
        public string CombinedIssuance { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the type of the credential.
        /// </summary>
        public string CredentialType { get; set; } = null!;

        /// <summary>
        ///     Gets or sets the identifier for the issuer.
        /// </summary>
        public string IssuerId { get; set; } = null!;

        /// <inheritdoc />
        public override string TypeName => "AF.SdJwtRecord";

        /// <summary>
        ///     Gets or sets the key record ID.
        /// </summary>
        public string? KeyId { get; set; }

        /// <summary>
        ///     Creates a SdJwtRecord from a SdJwtDoc.
        /// </summary>
        /// <param name="sdJwtDoc">The SdJwtDoc.</param>
        /// <returns>The SdJwtRecord.</returns>
        public static SdJwtRecord FromSdJwtDoc(SdJwtDoc sdJwtDoc)
        {
            var record = new SdJwtRecord
            {
                CombinedIssuance = sdJwtDoc.EncodedJwt,
                Claims = CreateClaimsDictionary(sdJwtDoc.Disclosures),
                CredentialType = ExtractTypeFromJwtPayload(sdJwtDoc.EncodedJwt)
            };

            return record;
        }

        /// <summary>
        ///     Sets display properties of the SdJwtRecord based on the provided issuer metadata.
        /// </summary>
        /// <param name="issuerMetadata">The issuer metadata.</param>
        public void SetDisplayFromIssuerMetadata(OidIssuerMetadata issuerMetadata)
        {
            var credentialFormatAndType = new OidCredentialFormatAndType
            {
                Format = "vc+sd-jwt",
                Type = CredentialType
            };

            SetCredentialDisplayProperties(issuerMetadata, credentialFormatAndType);
            SetOidIssuerDisplayProperties(issuerMetadata);
        }

        /// <summary>
        ///     Creates a dictionary of claims based on the list of disclosures.
        /// </summary>
        /// <param name="disclosures">The list of disclosures.</param>
        /// <returns>The dictionary of claims.</returns>
        private static Dictionary<string, string> CreateClaimsDictionary(IEnumerable<Disclosure> disclosures)
        {
            var claimsDictionary = new Dictionary<string, string>();
            foreach (var disclosure in disclosures)
                claimsDictionary[disclosure.Name] = disclosure.Value as string ?? string.Empty;

            return claimsDictionary;
        }

        /// <summary>
        ///     Creates a dictionary of the issuer name in different languages based on the issuer metadata.
        /// </summary>
        /// <param name="issuerMetadata">The issuer metadata.</param>
        /// <returns>The dictionary of the issuer name in different languages.</returns>
        private Dictionary<string, string>? CreateIssuerNameDictionary(OidIssuerMetadata issuerMetadata)
        {
            var issuerNameDictionary = new Dictionary<string, string>();

            foreach (var display in issuerMetadata.Display?.Where(d => d.Locale != null) ?? Enumerable.Empty<OidIssuerDisplay>())
                issuerNameDictionary[display.Locale!] = display.Name!;

            return issuerNameDictionary.Count > 0 ? issuerNameDictionary : null;
        }

        /// <summary>
        ///     Extracts the "type" property from the JWT payload.
        /// </summary>
        /// <param name="encodedJwt">The encoded JWT.</param>
        /// <returns>The value of the "type" property.</returns>
        private static string ExtractTypeFromJwtPayload(string encodedJwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(encodedJwt);
            var payloadJson = jwtToken.Payload.SerializeToJson();
            var payloadObj = JsonDocument.Parse(payloadJson).RootElement;

            if (payloadObj.TryGetProperty("type", out var typeValue))
                return typeValue.GetString() ?? string.Empty;

            return string.Empty;
        }

        /// <summary>
        ///     Sets display properties related to the credential based on the issuer metadata.
        /// </summary>
        /// <param name="issuerMetadata">The issuer metadata.</param>
        /// <param name="credentialFormatAndType">The credential format and type.</param>
        private void SetCredentialDisplayProperties(OidIssuerMetadata issuerMetadata, OidCredentialFormatAndType credentialFormatAndType)
        {
            Display = issuerMetadata.GetCredentialDisplay(credentialFormatAndType);
            DisplayedAttributes = issuerMetadata.GetCredentialSubject(credentialFormatAndType);
        }

        /// <summary>
        ///     Sets display properties related to the issuer based on the issuer metadata.
        /// </summary>
        /// <param name="issuerMetadata">The issuer metadata.</param>
        private void SetOidIssuerDisplayProperties(OidIssuerMetadata issuerMetadata)
        {
            IssuerId = issuerMetadata.CredentialIssuer;
            IssuerName = CreateIssuerNameDictionary(issuerMetadata);
        }
    }
}
