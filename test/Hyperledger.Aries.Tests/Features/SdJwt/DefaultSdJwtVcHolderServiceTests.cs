using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperledger.Aries.Features.OpenId4Vc.Vp.Models;
using Hyperledger.Aries.Features.Pex.Models;
using Hyperledger.Aries.Features.SdJwt.Models.Records;
using Hyperledger.Aries.Features.SdJwt.Services.SdJwtVcHolderService;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Storage.Models.Interfaces;
using Hyperledger.Aries.Tests.Extensions;
using Moq;
using SD_JWT;
using Xunit;

namespace Hyperledger.Aries.Tests.Features.SdJwt
{
    public class DefaultSdJwtVcHolderServiceTests
    {
        [Fact]
        public async Task Can_Get_Credential_Candidates_For_Input_Descriptors()
        {
            // Arrange
            var driverLicenseClaims = new Dictionary<string, string>
            {
                { "id", "123" },
                { "issuer", "did:example:gov" },
                { "dateOfBirth", "01/01/2000" }
            };

            var universityCredentialClaims = new Dictionary<string, string>
            {
                { "degree", "Master of Science" },
                { "universityName", "ExampleUniversity" }
            };

            var driverLicenseCredential = CreateCredential(driverLicenseClaims);
            var driverLicenseCredentialClone = CreateCredential(driverLicenseClaims);
            var universityCredential = CreateCredential(universityCredentialClaims);

            var idFilter = new Filter();
            idFilter.PrivateSet(x => x.Type, "string");
            idFilter.PrivateSet(x => x.Const, "123");

            var driverLicenseInputDescriptor = CreateInputDescriptor(
                CreateConstraints(new[]
                    { CreateField("$.id", idFilter), CreateField("$.issuer"), CreateField("$.dateOfBirth") }),
                new Dictionary<string, Format> { {"vc+sd-jwt", CreateFormat(new[] { "ES256" }) }},
                Guid.NewGuid().ToString(),
                "EU Driver's License",
                "We can only accept digital driver's licenses issued by national authorities of member states or trusted notarial auditors.",
                new [] { "A" });

            var universityInputDescriptor = CreateInputDescriptor(
                CreateConstraints(new[] { CreateField("$.degree") }),
                new Dictionary<string, Format> { {"vc+sd-jwt", CreateFormat(new[] { "ES256" }) }},
                Guid.NewGuid().ToString(),
                "University Degree",
                "We can only accept digital university degrees.");

            var expected = new List<CredentialCandidates>
            {
                new CredentialCandidates
                {
                    InputDescriptorId = driverLicenseInputDescriptor.Id,
                    Group = driverLicenseInputDescriptor.Group ?? new[] { "A" },
                    Credentials = new List<ICredential> { driverLicenseCredential, driverLicenseCredentialClone }
                },
                new CredentialCandidates
                {
                    InputDescriptorId = universityInputDescriptor.Id,
                    Credentials = new List<ICredential> { universityCredential }
                }
            };

            var sdJwtVcHolderService = CreateSdJwtVcHolderService();

            // Act
            var credentialCandidatesArray = await sdJwtVcHolderService.GetCredentialCandidates(
                new[]
                {
                    driverLicenseCredential, driverLicenseCredentialClone, universityCredential
                },
                new [] { driverLicenseInputDescriptor, universityInputDescriptor });

            // Assert
            credentialCandidatesArray.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Cant_Get_Credential_Candidates_When_Not_All_Fields_Are_Fulfilled()
        {
            // Arrange
            var driverLicenseClaims = new Dictionary<string, string>
            {
                { "id", "123" },
                { "issuer", "did:example:gov" },
                { "dateOfBirth", "01/01/2000" }
            };

            var employeeCredential = CreateCredential(driverLicenseClaims);

            var driverLicenseInputDescriptor = CreateInputDescriptor(
                CreateConstraints(new[]
                {
                    CreateField("$.id"), CreateField("$.issuer"),
                    CreateField("$.dateOfBirth"), CreateField("$.name")
                }),
                new Dictionary<string, Format> { {"vc+sd-jwt", CreateFormat(new[] { "ES256" }) }},
                Guid.NewGuid().ToString(),
                "EU Driver's License",
                "We can only accept digital driver's licenses issued by national authorities of member states or trusted notarial auditors.");

            var sdJwtVcHolderService = CreateSdJwtVcHolderService();

            // Act
            var credentialCandidatesArray = await sdJwtVcHolderService.GetCredentialCandidates(
                new[] { employeeCredential },
                new[] { driverLicenseInputDescriptor });

            // Assert
            credentialCandidatesArray.Should().BeEmpty();
        }

        [Fact]
        public async Task Cant_Get_Credential_Candidates_When_Not_All_Filters_Are_Fulfilled()
        {
            // Arrange
            var driverLicenseClaims = new Dictionary<string, string>
            {
                { "id", "123" },
                { "issuer", "did:example:gov" },
                { "dateOfBirth", "01/01/2000" }
            };

            var driverLicenseCredential = CreateCredential(driverLicenseClaims);

            var idFilter = new Filter();
            idFilter.PrivateSet(x => x.Type, "string");
            idFilter.PrivateSet(x => x.Const, "326");

            var driverLicenseInputDescriptor = CreateInputDescriptor(
                CreateConstraints(new[]
                {
                    CreateField("$.id", idFilter), CreateField("$.issuer"), CreateField("$.dateOfBirth")
                }),
                new Dictionary<string, Format> { {"vc+sd-jwt", CreateFormat(new[] { "ES256" }) }},
                Guid.NewGuid().ToString(),
                "EU Driver's License",
                "We can only accept digital driver's licenses issued by national authorities of member states or trusted notarial auditors.");

            var sdJwtVcHolderService = CreateSdJwtVcHolderService();

            // Act
            var credentialCandidatesArray = await sdJwtVcHolderService.GetCredentialCandidates(
                new[] { driverLicenseCredential },
                new[] { driverLicenseInputDescriptor });

            // Assert
            credentialCandidatesArray.Should().BeEmpty();
        }

        private static Constraints CreateConstraints(Field[] fields)
        {
            var constraints = new Constraints();
            constraints.PrivateSet(x => x.Fields, fields);

            return constraints;
        }

        private static SdJwtRecord CreateCredential(Dictionary<string, string> claims)
        {
            var record = new SdJwtRecord
            {
                Id = Guid.NewGuid().ToString(),
                Claims = claims
            };

            return record;
        }

        private static Field CreateField(string path, Filter? filter = null)
        {
            var field = new Field();
            field.PrivateSet(x => x.Path, new[] { path });

            if (filter != null)
            {
                field.PrivateSet(x => x.Filter, filter);
            }

            return field;
        }

        private static Format CreateFormat(string[] supportedAlg)
        {
            var format = new Format();
            format.PrivateSet(x => x.Alg, supportedAlg);

            return format;
        }

        private static InputDescriptor CreateInputDescriptor(Constraints constraints, Dictionary<string, Format> formats, string id,
            string name, string purpose, string[]? group = null)
        {
            var inputDescriptor = new InputDescriptor();

            inputDescriptor.PrivateSet(x => x.Constraints, constraints);
            inputDescriptor.PrivateSet(x => x.Formats, formats);
            inputDescriptor.PrivateSet(x => x.Id, id);
            inputDescriptor.PrivateSet(x => x.Name, name);
            inputDescriptor.PrivateSet(x => x.Purpose, purpose);

            if (group != null)
            {
                inputDescriptor.PrivateSet(x => x.Group, group);
            }

            return inputDescriptor;
        }

        private static ISdJwtVcHolderService CreateSdJwtVcHolderService()
        {
            var holder = new Holder();
            var walletRecordService = new Mock<IWalletRecordService>();

            return new DefaultSdJwtVcHolderService(holder, walletRecordService.Object);
        }
    }
}
