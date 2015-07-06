using System;
using FhirNetApiExtension;
using Hl7.Fhir.Rest;
using System.Linq;
using Monads.NET;

namespace Lis.Test.Integration.Common
{
    public class Constants
    {
        //public const string Endpoint = "http://fhir-demo.zdrav.netrika.ru/fhir";

        public const string Endpoint = "http://localhost:50883/fhir";

        public const string TestToken = "a24f04ec-dbe0-4cdf-a87f-a6c1d98bfa43";

        public const string TerminologyEndpoint = "http://localhost:50883/term";

        public const string DictionaryEndpoint = "http://localhost:50883/dict";

        public const string Mkb10DictionaryName = "mkb10";

        public const string Mkb10DictionarySystem = "http://netrika.ru/mkb10-inner";

        public static string LisOrderStructureDefinitionReference
        {
            get { return GetOrCreateStructureDefinition("LisOrder"); }
        }

        public static string LisResultStructureDefinitionReference
        {
            get { return GetOrCreateStructureDefinition("LisResult"); }
        }

        private static readonly Lazy<N3FhirClient> LazyClient = new Lazy<N3FhirClient>(() => new N3FhirClient(Endpoint, new N3Credentials(TestToken)));

        public static N3FhirClient FhirClient
        {
            get { return LazyClient.Value; }
        }

        private static string GetOrCreateStructureDefinition(string structureDefinitionName)
        {
            var sdSearchParams = new SearchParams();
            sdSearchParams.Add("name", structureDefinitionName);
            var sd = FhirClient.Search<Hl7.Fhir.Model.StructureDefinition>(sdSearchParams);

            var sdReference = sd
                .With(x => x.Entry)
                .With(x => x.FirstOrDefault())
                .With(x => FhirHelper.CreateReference(x.Resource));

            if (sdReference == null)
            {
                var structureDefinition = new Hl7.Fhir.Model.StructureDefinition {Name = structureDefinitionName};
                var createdSd = FhirClient.Create(structureDefinition);
                return FhirHelper.CreateReference(createdSd).Reference;
            }
            else
            {
                return sdReference.Reference;
            }
        }
    }
}