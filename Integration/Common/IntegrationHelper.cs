using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Hl7.Fhir.Model;
using Monads.NET;

namespace Lis.Test.Integration.Common
{
    public class IntegrationHelper
    {
        public static Bundle CreateRandomLabOrder()
        {
            var patient = FhirResourceHelper.CreatePatient();

            var orderOrganization = FhirResourceHelper.CreateOrganization();
            var diagnosticOrderOrganization = FhirResourceHelper.CreateOrganization();

            var orderPractitioner = FhirResourceHelper.CreatePractitioner(orderOrganization);
            var diagnosticOrderPractitioner = FhirResourceHelper.CreatePractitioner(diagnosticOrderOrganization);

            var specimen = FhirResourceHelper.GetSpecimen(patient);
            var specimen1 = FhirResourceHelper.GetSpecimen(patient);

            var condition = FhirResourceHelper.GetCondition(patient);

            var encounter = FhirResourceHelper.CreateEncounter(patient, condition, orderOrganization);

            var observation = FhirResourceHelper.GetObservation();
            var observation1 = FhirResourceHelper.GetObservation();

            var diagnosticOrder = FhirResourceHelper.GetDiagnosticOrder(patient, diagnosticOrderPractitioner, encounter, 
                observation, observation1, specimen, specimen1);
            var order = FhirResourceHelper.GetOrder(patient, orderPractitioner, orderOrganization, diagnosticOrder);
            var lisOrderBundle = FhirResourceHelper.GetLisOrderBundle(order, diagnosticOrder, condition, 
                new List<Specimen>{specimen, specimen1},
                new List<Observation>{observation, observation1});

            return lisOrderBundle;
        }


        public static string GetSpecimenBarcode(Bundle orderResponse)
        {
            var component = orderResponse.Entry.FirstOrDefault(x => x.Resource is Order);

            var order = (Order) component.Resource;
            var diagnosticOrder = FhirResourceHelper.Read<DiagnosticOrder>(order.Detail.FirstOrDefault().Reference);
            var specRef = diagnosticOrder.Specimen.FirstOrDefault().With(x => x.Reference);
            var spec = FhirResourceHelper.Read<Specimen>(specRef);
            return spec.Container.FirstOrDefault().Identifier.FirstOrDefault().Value;
        }

        public static Order GetOrderByBarcode(string specimenBarcode)
        {
            var specimen = FhirResourceHelper.SpecimenByBarcode(specimenBarcode);
            var diagnosticOrder = FhirResourceHelper.SearchDiagnosticOrderBySpecimen(specimen);
            var order = FhirResourceHelper.GetOrderByDiagnosticOrder(diagnosticOrder);

            return order;
        }

        public static Bundle CreateLabResultForOrder(Order order)
        {
            var resultOrganization = order.Target;

            var practitioner1 = FhirResourceHelper.GetPractitioner(resultOrganization);
            var practitioner2 = FhirResourceHelper.GetPractitioner(resultOrganization);
            var reportPractitioner = FhirResourceHelper.GetPractitioner(resultOrganization);

            var observation1 = FhirResourceHelper.GetObservation(practitioner1);
            var observation2 = FhirResourceHelper.GetObservation(practitioner2);

            var diagnosticReport = FhirResourceHelper.GetDiagnosticReport(order.Subject, 
                reportPractitioner, observation1, observation2, order.Detail.First());

            var orderResponse = FhirResourceHelper.GetOrderResponse(order, new[] {diagnosticReport});

            var lisResultBundle = new Bundle
            {
                Meta = new Meta
                {
                    Profile = new List<string> { Constants.LisResultStructureDefinitionReference },
                },
                Type = Bundle.BundleType.Transaction,
                Entry = new List<Bundle.BundleEntryComponent>
                {
                    new Bundle.BundleEntryComponent
                    {
                        Resource = orderResponse,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = orderResponse.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = practitioner1,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = practitioner1.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = practitioner2,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = practitioner2.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = reportPractitioner,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = reportPractitioner.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = diagnosticReport,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = diagnosticReport.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = observation1,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = observation1.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = observation2,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = observation2.TypeName,
                        }
                    },
                }
            };

            return lisResultBundle;
        }

        public static List<Tuple<string, string>> GetOrderCodes(Bundle orderResponse)
        {
            var barcode = GetSpecimenBarcode(orderResponse);
            var order = GetOrderByBarcode(barcode);
            var organizarion = FhirResourceHelper.Read<Organization>(order.Target.Reference);

            return new List<Tuple<string, string>>
            {
                new Tuple<string, string>(organizarion.Identifier.First().Value, barcode),
            };
        }

        public static Tuple<string, string> GetResultCodes(Order order)
        {
            var orderMisId = order.Identifier
                .FirstOrDefault()
                .If(x => x.System == "http://netrika.ru")
                .With(x => x.Value);

            var targetOrganization = FhirResourceHelper.Read<Organization>(order.Target.Reference);
            var targetCode = targetOrganization.Identifier.First().Value;

            return new Tuple<string, string>(targetCode, orderMisId);
        }

        public static Order GetOrder(Bundle orderResponse)
        {
            //var orderCodes = GetOrderCodes(orderResponse).First();
            //var getOrderResponse = FhirResourceHelper.GetOrderOperation(targetCode: orderCodes.Item1,
            //        barcode: orderCodes.Item2);
            //var order = (Order)getOrderResponse.Parameter.Find(x => x.Name == "Order").Resource;

            var order = orderResponse.Entry
                .FirstOrDefault(x => x.Resource is Order)
                .With(x => (Order)x.Resource);

            return order;
        }
    }
}