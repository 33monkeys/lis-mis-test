using System;
using System.Linq;
using Hl7.Fhir.Rest;
using Monads.NET;

namespace Lis.Test.Integration.Common
{
    public class Constants
    {
        public const string Endpoint = "http://fhir-demo.zdrav.netrika.ru/fhir";
        //public const string Endpoint = "http://localhost:50883/fhir";

        public static string LisOrderStructureDefinitionReference
        {
            get { return GetOrCreateStructureDefinition("LisOrder"); }
        }

        public static string LisResultStructureDefinitionReference
        {
            get { return GetOrCreateStructureDefinition("LisResult"); }
        }

        private static readonly Lazy<FhirClient> LazyClient = new Lazy<FhirClient>(() => new FhirClient(Endpoint));

        public static FhirClient FhirClient
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