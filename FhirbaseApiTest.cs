using System.Collections.Generic;
using System.IO;
using Fhirbase.Net;
using Fhirbase.Net.Api;
using Fhirbase.Net.Common;
using NUnit.Core;
using NUnit.Framework;

namespace Lis.Test
{
    [TestFixture]
    class FhirbaseApiTest
    {
        public List<string> Bundles = new List<string>
        {
            File.ReadAllText("Transaction/valid-transaction.json"),
            File.ReadAllText("Transaction/valid-transaction1.json"),
            File.ReadAllText("Transaction/valid-transaction2.json"),
        };

        public IFHIRbase FhirbaseApi { get; set; }

        [TestFixtureSetUp]
        public void InitBundles()
        {
            FhirbaseApi = new FHIRbaseApi();
        }

        [Test]
        public void TransactionTest([ValueSource("Bundles")] string transactionBundleJson)
        {
            Assert.That(transactionBundleJson, Is.Not.Null.Or.Empty);

            var transactionBundle = FHIRbaseHelper.JsonToBundle(transactionBundleJson);

            Assert.That(transactionBundle, Is.Not.Null);

            var result = FhirbaseApi.Transaction(transactionBundle);

            Assert.That(result, Is.Not.Null);
        }
    }
}
