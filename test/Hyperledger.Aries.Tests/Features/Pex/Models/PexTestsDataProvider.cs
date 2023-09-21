using System;
using System.IO;
using System.Reflection;

namespace Hyperledger.Aries.Tests.Features.Pex.Models
{
    public static class PexTestsDataProvider
    {
        public static string GetPresentationDefinitionJson()
        {
            return GetJson("PresentationDefinition.json");
        }
        
        public static string GetInputDescriptorsJson()
        {
            return GetJson("InputDescriptors.json");
        }

        private static string GetJson(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var currentNamespace = typeof(PexTestsDataProvider).Namespace;
            var resourceName = $"{currentNamespace}.{fileName}";

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
