using System;
using System.IO;
using System.Reflection;

namespace Hyperledger.Aries.Tests.Features.Pex.Models
{
    public static class PexDataProvider
    {
        public static string GetInputDescriptorsJson()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var currentNamespace = typeof(PexDataProvider).Namespace;
            var resourceName = $"{currentNamespace}.InputDescriptors.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException($"Could not find resource with name {resourceName}");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
