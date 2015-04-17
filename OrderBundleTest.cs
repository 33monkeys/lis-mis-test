using System;
using System.IO;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using NUnit.Framework;

namespace Lis.Test
{
    [TestFixture]
    public class OrderBundleTest
    {
        public Bundle OrderBundle { get; set; }

        private const string OrderTransactionBundlePath = "OrderTransactionBundle.json";

        [TestFixtureSetUp]
        public void Init()
        {
            var bundleJson = File.ReadAllText(OrderTransactionBundlePath);
            var bundle = FhirParser.ParseFromJson(bundleJson);
            OrderBundle = (Bundle) bundle;
        }

        [TestCase]
        public void CreateOrderTransaction()
        {
        }
    }
}
