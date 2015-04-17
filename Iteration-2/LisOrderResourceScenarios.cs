//using Hl7.Fhir.Model;
//using NUnit.Framework;

//namespace Lis.Test
//{
//    partial class LisOrderScenarios
//    {
//        [ExpectedException]
//        [Category("Invalid resource")]
//        [TestCase("Invalid/Organization.json")]
//        [TestCase("Invalid/DiagnosticOrder.json")]
//        [TestCase("Invalid/Encounter.json")]
//        [Description("Создает ресурсы на сервере не проходящие валидацию")]
//        public void Create_Invalid_Resources(string resource)
//        {
//            var rs = LoadResource<Resource>(resource);
//            Client.Create(rs);
//        }

//        [Category("Valid resource")]
//        [TestCaseSource("ValidResources")]
//        [Description("Создает ресурсы на сервере, успешно проходящие валидацию")]
//        public void Create_Valid_Resources(Resource resource)
//        {
//            Assert.That(resource.VersionId, Is.Null.Or.Empty);

//            var result = Client.Create(resource);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.VersionId, Is.Not.Null.Or.Empty);
//        }
//    }
//}
