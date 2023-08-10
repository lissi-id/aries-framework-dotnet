using System;
using System.Web;

namespace Hyperledger.Aries.Features.OpenId4VerifiablePresentation.Helpers
{
    public static class QueryParamUtils
    {
        public static bool HasQueryParam(this Uri uri, string param)
        {
            if (uri.ToString().Contains("%"))
                uri = new Uri(Uri.UnescapeDataString(uri.ToString()));

            var query = HttpUtility.ParseQueryString(uri.Query);

            return query[param] != null;
        }
        
        public static string GetQueryParam(this Uri uri, string param)
        {
            if (uri.ToString().Contains("%"))
                uri = new Uri(Uri.UnescapeDataString(uri.ToString()));

            var query = HttpUtility.ParseQueryString(uri.Query);

            return query[param];
        }
    }
}
