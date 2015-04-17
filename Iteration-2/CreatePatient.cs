using System.IO;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Lis.Test.Integration.Common;
using NUnit.Framework;

namespace Lis.Test
{
    [TestFixture]
    class CreatePatient
    {
        #region Props

        public Patient Patient { get; set; }

        private const string PatientPath = "Iteration-2/Data/Patient.json";

        private const string Endpoint = "http://localhost:50883/fhir";

        public FhirClient Client { get; private set; }

        #endregion

        [TestFixtureSetUp]
        public void Init()
        {
            Client = new FhirClient(Endpoint);
            //var patients = Client.Search<Patient>();
            //foreach (var component in patients.Entry)
            //    Client.Delete(component.Resource);

            //var patientJson = File.ReadAllText(PatientPath);
            //Patient = (Patient)FhirParser.ParseResourceFromJson(patientJson);
        }

        [Test]
        public void Test_Patient_CRUD()
        {
            var createdPatient = FhirResourceHelper.CreatePatient();
            Assert.That(createdPatient, Is.Not.Null);
            Assert.That(createdPatient.VersionId, Is.Not.Empty);

            var patient = Client.Read<Patient>(string.Format("Patient/{0}", createdPatient.Id));
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.VersionId, Is.Not.Empty);

            createdPatient.Name.Add(HumanName.ForFamily("Kramer").WithGiven("Hello"));
            var updatedPatient = Client.Update(createdPatient);

            Assert.That(createdPatient.VersionId, Is.Not.EqualTo(updatedPatient.VersionId));
            Assert.That(updatedPatient.Name.Exists(x => x.Given.Any(y => y == "Hello")), Is.True);

            Client.Delete(updatedPatient);
        }
    }
}
