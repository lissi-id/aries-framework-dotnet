using System.Linq;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Helpers
{
    internal static class StringUtils
    {
        internal static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
        }
    }
}
