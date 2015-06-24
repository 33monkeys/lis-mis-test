using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Lis.Test.Integration.Common;
using NUnit.Framework;

namespace Lis.Test.Integration
{
    [TestFixture]
    internal class StatusTest
    {
        public List<Bundle> Orders { get; set; }

        public FhirClient FhirClient { get; set; }

        public StatusTest()
        {
            Orders = new List<Bundle>();
            for (var i = 0; i < 1; i++)
                Orders.Add(IntegrationHelper.CreateRandomLabOrder());

            FhirClient = new FhirClient(Constants.Endpoint);
        }

        [TestCaseSource("Orders")]
        public void DiagnosticOrder_Status_Chain(Bundle lisOrder)
        {
            var orderResponse = FhirClient.Transaction(lisOrder);
            var diagnosticOrders = orderResponse
                .Entry
                .Where(x => x.Resource.ResourceType == ResourceType.DiagnosticOrder)
                .Select(x => x.Resource)
                .Cast<DiagnosticOrder>();

            foreach (var diagnosticOrder in diagnosticOrders)
            {

            }
        }

        [TestCaseSource("Orders")]
        public void Encounter_Status_Chain(Bundle lisOrder)
        {

        }

        [TestCaseSource("Orders")]
        public void OrderResponse_Status_Chain(Bundle lisOrder)
        {

        }
    }
}
