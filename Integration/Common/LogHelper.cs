using System.IO;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace Lis.Test.Integration.Common
{
    public class LogHelper
    {
        public static void SaveBundle(Bundle lisOrder, string fullName)
        {
            throw new System.NotImplementedException();
        }

        public static void SaveOrderBundle(Bundle lisOrder, string resourceId)
        {
            if (!Directory.Exists("orders"))
                Directory.CreateDirectory("orders");

            var path = Path.ChangeExtension(Path.Combine("orders", resourceId), ".json");

            if (File.Exists(path))
                File.Delete(path);

            var json = FhirSerializer.SerializeToJson(lisOrder);
            File.WriteAllText(path, json);
        }

        public static void SaveResultBundle(Bundle lisOrder, string resourceId)
        {
            if (!Directory.Exists("results"))
                Directory.CreateDirectory("results");

            var path = Path.ChangeExtension(Path.Combine("results", resourceId), ".json");

            if (File.Exists(path))
                File.Delete(path);

            var json = FhirSerializer.SerializeToJson(lisOrder);
            File.WriteAllText(path, json);
        }

        public static void Clear()
        {
            if (Directory.Exists("orders"))
                foreach (var file in Directory.EnumerateFiles("orders"))
                    File.Delete(file);

            if (Directory.Exists("results"))
                foreach (var file in Directory.EnumerateFiles("results"))
                    File.Delete(file);
        }
    }
}