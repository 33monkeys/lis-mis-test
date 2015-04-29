using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Lis.Business.Helpers;
using Monads.NET;
using NUnit.Framework;
using RestSharp;

namespace Lis.Test.Integration.Common
{
    public class FhirResourceHelper
    {
        public static FhirClient FhirClient { get; set; }

        static FhirResourceHelper()
        {
            FhirClient = new FhirClient(Constants.Endpoint);
        }

        public static Patient CreatePatient()
        {
            var patient = new Patient
            {
                Gender = AdministrativeGender.Male,
                BirthDate = DateTime.Today.AddYears(-54).ToString(CultureInfo.CurrentCulture),
            };
            return FhirClient.Create(patient);
        }

        public static Practitioner CreatePractitioner(Organization orderOrganization)
        {
            var practitioner = new Practitioner
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.PRACTITIONER_IDENTIFIER, Guid.NewGuid().ToString())
                },
                Name = new HumanName
                {
                    Family = new List<string> {"Петров"},
                    Given = new List<string> {"Петр"}
                },
                PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
                {
                    new Practitioner.PractitionerPractitionerRoleComponent
                    {
                        ManagingOrganization = FhirHelper.CreateReference(orderOrganization),
                        Role = new CodeableConcept(Systems.PRACTITIONER_ROLE, Guid.NewGuid().ToString()),
                        Specialty = new List<CodeableConcept>
                        {
                            new CodeableConcept(Systems.PRACTITIONER_SPECIALITY, Guid.NewGuid().ToString())
                        }
                    }
                }
            };

            return FhirClient.Create(practitioner);
        }

        public static Specimen GetSpecimen(Patient patient)
        {
            var specimen = new Specimen
            {
                Id = Guid.NewGuid().ToString(),
                Collection = new Specimen.SpecimenCollectionComponent
                {
                    Collected = new Date(DateTime.Today.ToString(CultureInfo.CurrentCulture))
                },
                Type = new CodeableConcept(Systems.BIOMATERIAL, Guid.NewGuid().ToString()),
                Container = new List<Specimen.SpecimenContainerComponent>
                {
                    new Specimen.SpecimenContainerComponent
                    {
                        Identifier = new List<Identifier>
                        {
                            new Identifier(Systems.CONTAINER_TYPE_IDENTIFIER, Guid.NewGuid().ToString())
                        },
                        Type = new CodeableConcept(Systems.CONTAINER_TYPE, Guid.NewGuid().ToString())
                    }
                },
                Subject = FhirHelper.CreateReference(patient)
            };

            return specimen;
        }

        public static Condition GetCondition(Patient patient)
        {
            var condition = new Condition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.MIS_IDENTIFIER, Guid.NewGuid().ToString())
                },
                Patient = FhirHelper.CreateReference(patient),
                DateAsserted = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Code = new CodeableConcept(Systems.MKB10, Guid.NewGuid().ToString()),
                Category = new CodeableConcept(Systems.CONDITION_CATEGORY, Guid.NewGuid().ToString()),
                ClinicalStatus = Condition.ConditionClinicalStatus.Confirmed,
                Notes = "Condition notes",
                //DueTo = new List<Condition.ConditionDueToComponent>
                //{
                //    new Condition.ConditionDueToComponent
                //    {
                //        Target = 
                //    }
                //}
            };

            return condition;
        }

        public static Encounter CreateEncounter(Patient patient, Condition condition)
        {
            var encounter = new Encounter
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.MIS_IDENTIFIER, Guid.NewGuid().ToString())
                },
                Type = new List<CodeableConcept>
                {
                    new CodeableConcept(Systems.ENCOUNTER_TYPE, Guid.NewGuid().ToString())
                },
                Class = Encounter.EncounterClass.Outpatient,
                Status = Encounter.EncounterState.Planned,
                Patient = FhirHelper.CreateReference(patient),
                Indication = new List<ResourceReference> {new ResourceReference{Reference = condition.Id}},
                Reason = new List<CodeableConcept>
                {
                    new CodeableConcept(Systems.REASON_CODE, Guid.NewGuid().ToString())
                }
            };

            return FhirClient.Create(encounter);
        }

        public static Organization CreateOrganization()
        {
            var organization = new Organization
            {
                Identifier = new List<Identifier>
                {
                    new Identifier("http://netrika", Guid.NewGuid().ToString()),
                }
            };
            return FhirClient.Create(organization);
        }

        public static DiagnosticOrder GetDiagnosticOrder(Patient patient, Practitioner diagnosticOrderPractitioner, 
            Encounter encounter, 
            Observation observation, Observation observation1, 
            Specimen specimen, Specimen specimen1)
        {
            var diagnosticOrder = new DiagnosticOrder
            {
                Id = Guid.NewGuid().ToString(),
                Subject = FhirHelper.CreateReference(patient),
                Orderer = FhirHelper.CreateReference(diagnosticOrderPractitioner),
                Encounter = FhirHelper.CreateReference(encounter),
                SupportingInformation = new List<ResourceReference>
                {
                    FhirHelper.CreateBundleReference(observation),
                    FhirHelper.CreateBundleReference(observation1),
                },
                Specimen = new List<ResourceReference>
                {
                    FhirHelper.CreateBundleReference(specimen),
                    FhirHelper.CreateBundleReference(specimen1)
                },
                Status = DiagnosticOrder.DiagnosticOrderStatus.Proposed,
                Item = new List<DiagnosticOrder.DiagnosticOrderItemComponent>
                {
                    new DiagnosticOrder.DiagnosticOrderItemComponent
                    {
                        Code = new CodeableConcept
                        {
                            Text = "Услуга 1",
                            Coding = new List<Coding>
                            {
                                new Coding(Systems.DIAGNOSTIC_ORDER_CODE, Guid.NewGuid().ToString()),
                            },
                            Extension = new List<Extension>
                            {
                                new Extension(Systems.DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION, new CodeableConcept(Systems.FINANCIAL, Guid.NewGuid().ToString())),
                                new Extension(Systems.DIAGNOSTIC_ORDER_INSURANCE_EXTENSION, new ResourceReference{Reference = Guid.NewGuid().ToString()})
                            }
                        },
                    }
                }
            };

            return diagnosticOrder;
        }

        public static Observation GetObservation()
        {
            var observation = new Observation
            {
                Id = Guid.NewGuid().ToString(),
                Code = new CodeableConcept(Systems.OBSERVATION_NAME, Guid.NewGuid().ToString()),
                Status = Observation.ObservationStatus.Registered,
                Value = new Quantity
                {
                    Value = 10,
                }
            };
            return observation;
        }

        public static Order GetOrder(Patient patient, Practitioner orderPractitioner, 
            Organization orderOrganization,
            Resource diagnosticOrder)
        {
            var order = new Order
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.MIS_IDENTIFIER, Guid.NewGuid().ToString()),
                },
                Date = DateTime.Today.ToString(CultureInfo.CurrentCulture),
                When = new Order.OrderWhenComponent
                {
                    Code = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding(Systems.PRIORIRY, Guid.NewGuid().ToString())
                        }
                    }
                },
                Subject = FhirHelper.CreateReference(patient),
                Source = FhirHelper.CreateReference(orderPractitioner),
                Target = FhirHelper.CreateReference(orderOrganization),
                Detail = new List<ResourceReference> {FhirHelper.CreateBundleReference(diagnosticOrder)},
            };

            return order;
        }

        public static Bundle GetLisOrderBundle(Resource order, Resource diagnosticOrder, Condition condition, 
            List<Specimen> specimens, 
            List<Observation> observations)
        {
            var lisOrderBundle = new Bundle
            {
                Meta = new Meta
                {
                    Profile = new List<string> { Constants.LisOrderStructureDefinitionReference },
                },
                Type = Bundle.BundleType.Transaction,
                Entry = new List<Bundle.BundleEntryComponent>
                {
                    new Bundle.BundleEntryComponent
                    {
                        Resource = diagnosticOrder,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = diagnosticOrder.TypeName,
                        }
                    },
                    new Bundle.BundleEntryComponent
                    {
                        Resource = order,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = order.TypeName
                        }
                    }, new Bundle.BundleEntryComponent
                    {
                        Resource = condition,
                        Transaction = new Bundle.BundleEntryTransactionComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = condition.TypeName
                        }
                    }
                }
            };

            foreach (var specimen in specimens)
            {
                lisOrderBundle.Entry.Add(new Bundle.BundleEntryComponent
                {
                    Resource = specimen,
                    Transaction = new Bundle.BundleEntryTransactionComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = specimen.TypeName
                    }
                });
            }

            foreach (var observation in observations)
            {
                lisOrderBundle.Entry.Add(new Bundle.BundleEntryComponent
                {
                    Resource = observation,
                    Transaction = new Bundle.BundleEntryTransactionComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = observation.TypeName
                    }
                });
            }

            return lisOrderBundle;
        }

        public static T Read<T>(string reference) where T: Resource
        {
            return FhirClient.Read<T>(reference);
        }

        public static void ClearTables()
        {
            ClearTable<Condition>();
            ClearTable<DiagnosticOrder>();
            ClearTable<Encounter>();
            ClearTable<Observation>();
            ClearTable<Order>();
            ClearTable<Organization>();
            ClearTable<Patient>();
            ClearTable<Practitioner>();
            ClearTable<Specimen>();
            ClearTable<OrderResponse>();
            ClearTable<DiagnosticReport>();
        }

        static void ClearTable<T>() where T : Resource, new()
        {
            var response = FhirClient.Search<T>();
            foreach (var component in response.Entry)
                FhirClient.Delete(component.Resource);
        }

        public static Specimen SpecimenByBarcode(string specimenBarcode)
        {
            var result = FhirClient.Search<Specimen>(new[] {string.Format("containerid={0}", specimenBarcode)});

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Entry, Is.Not.Null);
            Assert.That(result.Entry.Count, Is.EqualTo(1));

            return (Specimen) result.Entry.First().Resource;
        }

        public static DiagnosticOrder SearchDiagnosticOrderBySpecimen(Specimen specimen)
        {
            var result = FhirClient.Search<DiagnosticOrder>(new[] { string.Format("specimen={0}", FhirHelper.CreateReference(specimen).Reference) });
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Entry, Is.Not.Null);
            Assert.That(result.Entry.Count, Is.GreaterThanOrEqualTo(1));

            return (DiagnosticOrder) result.Entry.First().Resource;
        }

        public static Order GetOrderByDiagnosticOrder(DiagnosticOrder diagnosticOrder)
        {
            var result = FhirClient.Search<Order>(new[] { string.Format("detail={0}", FhirHelper.CreateReference(diagnosticOrder).Reference) });

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Entry, Is.Not.Null);
            Assert.That(result.Entry.Count, Is.GreaterThanOrEqualTo(1));

            return (Order)result.Entry.First().Resource;
        }

        public static Observation GetObservation(Practitioner practitioner)
        {
            return new Observation
            {
                Id = Guid.NewGuid().ToString(),
                Issued = DateTime.Now,
                Status = Observation.ObservationStatus.Amended,
                Performer = new List<ResourceReference> { FhirHelper.CreateReference(practitioner) },
                ReferenceRange = new List<Observation.ObservationReferenceRangeComponent>
                {
                    new Observation.ObservationReferenceRangeComponent
                    {
                        Low = new Quantity(),
                        High = new Quantity()
                    }
                }
            };
        }

        public static DiagnosticReport GetDiagnosticReport(ResourceReference orderSubject, Practitioner practitioner1,
            Observation observation1, Observation observation2)
        {
            return new DiagnosticReport
            {
                Id = Guid.NewGuid().ToString(),
                Subject = orderSubject,
                Conclusion = "Заключение",
                Status = DiagnosticReport.DiagnosticReportStatus.Final,
                Issued = DateTime.Today.AddDays(-1).ToString(CultureInfo.CurrentCulture),
                PresentedForm = new List<Attachment>
                {
                    new Attachment {Hash = Encoding.UTF8.GetBytes("hash"), Data = Encoding.UTF8.GetBytes("data")}
                },
                Performer = FhirHelper.CreateReference(practitioner1),
                Name = new CodeableConcept("Name", Guid.NewGuid().ToString()),
                Result = new List<ResourceReference>
                {
                    new ResourceReference {Reference = observation1.Id},
                    new ResourceReference {Reference = observation2.Id},
                }
            };
        }

        public static OrderResponse GetOrderResponse(Order order, DiagnosticReport diagnosticReport)
        {
            return new OrderResponse
            {
                Identifier = new List<Identifier> { new Identifier { Value = Guid.NewGuid().ToString() } },
                Date = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                OrderStatus_ = OrderResponse.OrderStatus.Completed,
                Request = FhirHelper.CreateReference(order),
                Fulfillment = new List<ResourceReference>
                {
                    new ResourceReference{Reference = diagnosticReport.Id}
                }
            };
        }

        public static Parameters GetOrderOperation(string sourceCode = null, string targetCode = null, string barcode = null)
        {
            var parameters = new Parameters();
            sourceCode.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "SourceCode",
                Value = new FhirString(x)
            }));

            targetCode.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "TargetCode",
                Value = new FhirString(x)
            }));

            barcode.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "Barcode",
                Value = new FhirString(x)
            }));

            var client = new RestClient(Constants.Endpoint);
            var request = new RestRequest("$getorder", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");

            var jsonToSend = FhirSerializer.SerializeToJson(parameters);
            request.Parameters.Clear();
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.AddParameter("_format", "json", ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            return (Parameters)FhirParser.ParseFromJson(response.Content);
        }

        public static Parameters GetResultOperation(string sourceCode = null, string targetCode = null, string orderMisId = null)
        {
            var parameters = new Parameters();
            sourceCode.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "SourceCode",
                Value = new FhirString(x)
            }));

            targetCode.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "TargetCode",
                Value = new FhirString(x)
            }));

            orderMisId.Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "OrderMisID",
                Value = new FhirString(x)
            }));

            var client = new RestClient(Constants.Endpoint);
            var request = new RestRequest("$getresult", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");

            var jsonToSend = FhirSerializer.SerializeToJson(parameters);
            request.Parameters.Clear();
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.AddParameter("_format", "json", ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            return (Parameters)FhirParser.ParseFromJson(response.Content);
        }
    }
}
