using System.Collections.Generic;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.OpenID4VP.Models;

public class CredentialCandidates
{
    public string InputDescriptorId {get; set;}
  
    public List<ICredential> Credentials {get; set;}
}
