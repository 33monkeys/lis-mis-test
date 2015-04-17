using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using NUnit.Framework;

namespace Lis.Test.StructureDefinition
{
    [TestFixture]
    class StructureDefinitionTest
    {
        private const string Endpoint = "http://localhost:50883/fhir";

        public FhirClient FhirClient { get; set; }

        [TestFixtureSetUp]
        public void Init()
        {
            FhirClient = new FhirClient(Endpoint);
        }

        [Test]
        public void CreateStructureDefinitions()
        {
            var misOrderStructureDefinition = new Hl7.Fhir.Model.StructureDefinition
            {
                Id = "LisOrder",
            };

            var lisResultStructureDefinition = new Hl7.Fhir.Model.StructureDefinition
            {
                Id = "LisResult",
            };

            FhirClient.Delete(string.Format("StructureDefinition/{0}", misOrderStructureDefinition.Id));
            FhirClient.Delete(string.Format("StructureDefinition/{0}", lisResultStructureDefinition.Id));

            FhirClient.Create(misOrderStructureDefinition);
            FhirClient.Create(lisResultStructureDefinition);
        }
    }
}
