//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Hl7.Fhir.Model;
//using Hl7.Fhir.Rest;
//using Hl7.Fhir.Serialization;
//using Lis.Test.Integration.Common;
//using Monads.NET;
//using NUnit.Framework;
//using NUnit.Framework.Constraints;
//using RestSharp;

//namespace Lis.Test
//{
//   // [TestFixture]
//    partial class LisOrderScenarios
//    {
//        [Test]
//        public void Create_Order_Response()
//        {
//            var order = CreateOrderForResponse();
//            var resultBundle = CreateResultBundle(order);
//            var response = Client.Transaction(resultBundle);

//            Assert.That(response, Is.Not.Null);
//        }

//        private Bundle CreateResultBundle(Order lisOrder)
//        {
//            var patient = Client.Read<Patient>(lisOrder.Subject.Reference);

//            // Врачи, выполнявшие тесты
//            var practitioner1 = Client.Create(LoadResource<Practitioner>("Practitioner.json"));
//            var practitioner2 = Client.Create(LoadResource<Practitioner>("Practitioner-1.json"));

//            var observation1 = new Observation
//            {
//                Id = Guid.NewGuid().ToString(),
//                Issued = DateTime.Now,
//                Status = Observation.ObservationStatus.Amended,
//                Performer = new List<ResourceReference>{FhirHelper.CreateReference(practitioner1)},
//                ReferenceRange = new List<Observation.ObservationReferenceRangeComponent>
//                {
//                    new Observation.ObservationReferenceRangeComponent
//                    {
//                        Low = new Quantity(),
//                        High = new Quantity()
//                    }
//                }
//            };

//            var observation2 = new Observation
//            {
//                Id = Guid.NewGuid().ToString(),
//                Issued = DateTime.Now,
//                Status = Observation.ObservationStatus.Amended,
//                Performer = new List<ResourceReference>
//                {
//                    FhirHelper.CreateReference(practitioner1), FhirHelper.CreateReference(practitioner2),
//                },
//                ReferenceRange = new List<Observation.ObservationReferenceRangeComponent>
//                {
//                    new Observation.ObservationReferenceRangeComponent
//                    {
//                        Low = new Quantity(),
//                        High = new Quantity()
//                    }
//                }
//            };

//            var diagnosticReport = new DiagnosticReport
//            {
//                Id = Guid.NewGuid().ToString(),
//                Subject = FhirHelper.CreateReference(patient),
//                Conclusion = "Заключение",
//                Status = DiagnosticReport.DiagnosticReportStatus.Final,
//                Issued = DateTime.Today.AddDays(-1).ToString(CultureInfo.CurrentCulture),
//                PresentedForm = new List<Attachment>
//                {
//                    new Attachment {Hash = Encoding.UTF8.GetBytes("hash"), Data = Encoding.UTF8.GetBytes("data")}
//                },
//                Performer = FhirHelper.CreateReference(practitioner1),
//                Name = new CodeableConcept("Name", "1.2.643.5.1.13.2.1.1.473"),
//                Result = new List<ResourceReference>
//                {
//                    new ResourceReference {Reference = observation1.Id},
//                    new ResourceReference {Reference = observation2.Id},
//                }
//            };

//            var orderResponse = new OrderResponse
//            {
//                Identifier = new List<Identifier>{new Identifier{Value = "12345"}},
//                Date = DateTime.Now.ToString(CultureInfo.CurrentCulture),
//                OrderStatus_ = OrderResponse.OrderStatus.Completed,
//                Request = FhirHelper.CreateReference(lisOrder),
//                Fulfillment = new List<ResourceReference>
//                {
//                    new ResourceReference{Reference = diagnosticReport.Id}
//                }
//            };

//            var lisResultBundle = new Bundle
//            {
//                Meta = new Meta
//                {
//                    Profile = new List<string> { GetLisResultDtructureDefinition().Reference },
//                },
//                Type = Bundle.BundleType.Transaction,
//                Entry = new List<Bundle.BundleEntryComponent>
//                {
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = orderResponse,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = orderResponse.TypeName,
//                        }
//                    },
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = diagnosticReport,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = diagnosticReport.TypeName,
//                        }
//                    },
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = observation1,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = observation1.TypeName,
//                        }
//                    },
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = observation2,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = observation2.TypeName,
//                        }
//                    },
//                }
//            };

//            return lisResultBundle;
//        }

//        private static ResourceReference GetLisResultDtructureDefinition()
//        {
//            var reference = string.Format("StructureDefinition/LisResult");
//            return new ResourceReference
//            {
//                Reference = reference
//            };
//        }

//        private Order CreateOrderForResponse()
//        {
//            var orderBundle = CreateLisOrderBundle();
//            var result = Client.Transaction(orderBundle);

//            var order = result.Entry.FirstOrDefault(x => x.Resource is Order);
//            Assert.That(order, Is.Not.Null);
//            Assert.That(order.Resource, Is.Not.Null);
//            Assert.That(order.Resource.Id, Is.Not.Null.Or.Empty);

//            return order.Resource as Order;
//        }
//    }
//}
