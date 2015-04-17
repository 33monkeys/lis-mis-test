//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Hl7.Fhir.Model;
//using Hl7.Fhir.Rest;
//using Hl7.Fhir.Serialization;
//using NUnit.Framework;
//using RestSharp;

//namespace Lis.Test
//{
//    partial class LisOrderScenarios
//    {
//        [Test]
//        [Category("Valid order")]
//        [Description("Сценарий, создающий валидную заявку на ЛИ с помощью бандла, содержащего все ресурсы")]
//        public void FullLisOrder_Full_Valid_Transaction()
//        {
//            var transactionBundle = LoadTransactionBundle("valid-transaction.json");
//            var result = Client.Transaction(transactionBundle);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Entry, Is.Not.Null);
//            Assert.That(result.Entry.Count, Is.GreaterThan(0));
//        }

//        [Test]
//        [Category("Valid order")]
//        [Description("Сценарий, создающий валидную заявку на ЛИ с помощью бандла, содержащего лишь Order и DiagnosticOrder")]
//        public void FullLisOrder_Order_DiagnosticOrder_Valid_Transaction()
//        {
//            var lisOrderBundle = CreateLisOrderBundle();
//            var result = Client.Transaction(lisOrderBundle);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Entry, Is.Not.Null);
//            Assert.That(result.Entry.Count, Is.GreaterThan(0));
//        }
//    }
//}
