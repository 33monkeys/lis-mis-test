using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Fhirbase.Net.Api;
using Fhirbase.Net.SearchHelpers;
using FhirNetApiExtension;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Lis.Test.Integration.Terminology;
using NUnit.Framework;
using Monads.NET;
using RestSharp;

namespace Lis.Test.Integration.Common
{
    public class FhirResourceHelper
    {
        public static N3FhirClient FhirClient { get; set; }

        public static IPostgresTerm PostgresTerm { get; set; }

        static FhirResourceHelper()
        {
            FhirClient = new N3FhirClient(Constants.Endpoint, new N3Credentials(Constants.TestToken));
            PostgresTerm = new PostgresTerm();
        }

        public static string GetCodeBySystem(string system)
        {
            return PostgresTerm.Search(system, null).Select(x => x.Code).First();
        }

        public static Patient CreatePatient()
        {
            var patient = new Patient
            {
                Identifier = new List<Identifier>{new Identifier(Systems.PATIENT_PASSPORT, GetCodeBySystem(Systems.PATIENT_PASSPORT))
                {
                    Assigner = new ResourceReference
                    {
                        Reference = "Организация 123"
                    }
                }},
                Gender = AdministrativeGender.Male,
                BirthDate = DateTime.Today.AddYears(-54).ToString(CultureInfo.CurrentCulture),
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = new List<string>{"Петров"},
                        Given = new List<string>{"Петр", "Петрович"}
                    }
                }
            };
            return FhirClient.Create(patient);
        }

        public static Practitioner CreatePractitioner(Organization orderOrganization)
        {
            var practitioner = new Practitioner
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS))
                    {
                        Assigner = FhirHelper.CreateReference(orderOrganization)
                    }
                },
                Name = new HumanName
                {
                    Family = new List<string> {"Врач"},
                    Given = new List<string> {"Врач"}
                },
                PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
                {
                    new Practitioner.PractitionerPractitionerRoleComponent
                    {
                        ManagingOrganization = FhirHelper.CreateReference(orderOrganization),
                        Role = new CodeableConcept(Systems.PRACTITIONER_ROLE, GetCodeBySystem(Systems.PRACTITIONER_ROLE)),
                        Specialty = new List<CodeableConcept>
                        {
                            new CodeableConcept(Systems.PRACTITIONER_SPECIALITY, GetCodeBySystem(Systems.PRACTITIONER_SPECIALITY))
                        }
                    }
                }
            };

            try
            {
                return FhirClient.Create(practitioner);

            }
            catch (FhirOperationException ex)
            {
                var st = ex.StackTrace;
                foreach (var operationOutcomeIssueComponent in ex.Outcome.Issue)
                {
                    
                }
            }

            return null;
        }

        public static Specimen GetSpecimen(Patient patient, Organization orderOrganization)
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
                            new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS))
                            {
                                Assigner = FhirHelper.CreateReference(orderOrganization)
                            }
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
                    new Identifier(Systems.MES, GetCodeBySystem(Systems.MES))
                },
                Patient = FhirHelper.CreateReference(patient),
                DateAsserted = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Code = new CodeableConcept(Systems.MKB10, GetCodeBySystem(Systems.MKB10)),
                Category = new CodeableConcept(Systems.CONDITION_CATEGORY, GetCodeBySystem(Systems.CONDITION_CATEGORY)),
                ClinicalStatus = Condition.ConditionClinicalStatus.Confirmed,
                Notes = "Condition notes",
            };

            return condition;
        }

        public static Encounter CreateEncounter(Patient patient, Condition condition, Organization orderOrganization)
        {
            var encounter = new Encounter
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS))
                    {
                        Assigner = FhirHelper.CreateReference(orderOrganization)
                    }
                },
                Type = new List<CodeableConcept>
                {
                    new CodeableConcept(Systems.ENCOUNTER_TYPE, GetCodeBySystem(Systems.ENCOUNTER_TYPE))
                },
                Class = Encounter.EncounterClass.Outpatient,
                Status = Encounter.EncounterState.Arrived,
                Patient = FhirHelper.CreateReference(patient),
                Indication = new List<ResourceReference> {new ResourceReference{Reference = condition.Id}},
                Reason = new List<CodeableConcept>
                {
                    new CodeableConcept(Systems.REASON_CODE, GetCodeBySystem(Systems.REASON_CODE))
                },
                ServiceProvider = FhirHelper.CreateReference(orderOrganization),
            };

            return FhirClient.Create(encounter);
        }

        public static Organization CreateOrganization()
        {
            var organization = new Organization
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS)),
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
                Status = DiagnosticOrder.DiagnosticOrderStatus.Requested,
                Item = new List<DiagnosticOrder.DiagnosticOrderItemComponent>
                {
                    new DiagnosticOrder.DiagnosticOrderItemComponent
                    {
                        Code = new CodeableConcept
                        {
                            Text = "Услуга 1",
                            Coding = new List<Coding>
                            {
                                new Coding(Systems.DIAGNOSTIC_ORDER_CODE, Systems.DIAGNOSTIC_ORDER_CODE),
                            },
                            Extension = new List<Extension>
                            {
                                new Extension(Systems.DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION, new CodeableConcept(Systems.FINANCIAL, GetCodeBySystem(Systems.FINANCIAL))),
                                new Extension(Systems.DIAGNOSTIC_ORDER_INSURANCE_EXTENSION, new ResourceReference{Reference = Guid.NewGuid().ToString()})
                            }
                        },
                    },
                    new DiagnosticOrder.DiagnosticOrderItemComponent
                    {
                        Code = new CodeableConcept
                        {
                            Text = "Услуга 2",
                            Coding = new List<Coding>
                            {
                                new Coding(Systems.DIAGNOSTIC_ORDER_CODE, Guid.NewGuid().ToString()),
                            },
                            Extension = new List<Extension>
                            {
                                new Extension(Systems.DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION, new CodeableConcept(Systems.FINANCIAL, GetCodeBySystem(Systems.FINANCIAL))),
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
                Code = new CodeableConcept(Systems.OBSERVATION_NAME, GetCodeBySystem(Systems.OBSERVATION_NAME)),
                Status = Observation.ObservationStatus.Final,
                Value = new Quantity
                {
                    Value = 10,
                },
                ReferenceRange = new List<Observation.ObservationReferenceRangeComponent>
                {
                    new Observation.ObservationReferenceRangeComponent
                    {
                        Text = "Test text",
                    }
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
                    new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS))
                    {
                        Assigner = FhirHelper.CreateReference(orderOrganization)
                    },
                },
                Date = DateTime.Today.ToString(CultureInfo.CurrentCulture),
                When = new Order.OrderWhenComponent
                {
                    Code = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding(Systems.PRIORIRY, GetCodeBySystem(Systems.PRIORIRY))
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
            Console.WriteLine("Deleting {0}", typeof(T));
            var api = new Fhirbase.Net.Api.FHIRbaseApi();
            var type = new T().ResourceType.ToString();
            while (true)
            {
                var response = api.Search(type, new SearchParameters());
                if (!response.Entry.Any())
                    break;

                foreach (var component in response.Entry)
                    api.Delete(component.Resource);
            }
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
                Code = new CodeableConcept(Systems.OBSERVATION_LOINC, Guid.NewGuid().ToString()),
                Comments = "Комментарии",
                Issued = DateTime.Now,
                Status = Observation.ObservationStatus.Final,
                Method = new CodeableConcept(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS)),
                Performer = new List<ResourceReference> { FhirHelper.CreateBundleReference(practitioner) },
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
            Observation observation1, Observation observation2, ResourceReference detail)
        {
            return new DiagnosticReport
            {
                Id = Guid.NewGuid().ToString(),
                Name = new CodeableConcept(Systems.DIAGNOSTIC_REPORT_CODE, GetCodeBySystem(Systems.DIAGNOSTIC_ORDER_CODE)),
                Status = DiagnosticReport.DiagnosticReportStatus.Final,
                Issued = DateTime.Today.AddDays(-1).ToString(CultureInfo.CurrentCulture),
                Subject = orderSubject,
                Performer = FhirHelper.CreateBundleReference(practitioner1),
                RequestDetail = new List<ResourceReference>{detail},
                //RequestDetail = new List<ResourceReference> { new ResourceReference { Reference = "/DiagnosticOrder/41b2a8eb-2447-47d6-92a1-286995276841" } },
                Conclusion = "Заключение",
                PresentedForm = new List<Attachment>
                {
                    new Attachment {Hash = Encoding.UTF8.GetBytes("hash"), Data = LoadSignedData()}
                },
                Result = new List<ResourceReference>
                {
                    FhirHelper.CreateBundleReference(observation1),
                    FhirHelper.CreateBundleReference(observation2),
                }
            };
        }

        private static byte[] LoadSignedData()
        {
            const string data = "eyJkYXRhIjoiPEVudmVsb3BlIHhtbG5zPVwiaHR0cDovL2hsNy5vcmcvZmhpclwiPjxwcmVzZW50ZWRGb3JtPkhlbGxvIHdvcmxkPC9wcmVzZW50ZWRGb3JtPjwvRW52ZWxvcGU+IiwicHVibGljX2tleSI6IkJpQUFBQ011QUFCTlFVY3hBQUlBQURBU0JnY3FoUU1DQWlRQUJnY3FoUU1DQWg0QkRST1BmbW1PZU9oODZWN2lDYXZDK2N2MEtPZVZEbmc4MlRnbWZhZGlMQWVtb1RQOTZYZWRhbEFpc2pEOHIrQW9Samg2QVZHdmFEZkFsa01penBzMTl3PT0iLCJoYXNoIjoiclFIVW0vVXgxNnFONy9Pc3dLeFNKM1c1OEpCY0hjS2JRMnhQRURmbkJ6OD0iLCJzaWduIjoiVktONjEreGp6UnNlbFUySXJuemo3aG9wOVMzY2MwOVNUdVpQNWhraW9hMzN3TitQRnZQc0Q1b21GUVNWN2pGMzFMellvTWYrY2VIWXE1RXlVVFpGQVE9PSJ9";
            return Encoding.UTF8.GetBytes(data);
        }

        private static void SignXml(XmlDocument xmlDoc, AsymmetricAlgorithm key)
        {
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (key == null)
                throw new ArgumentException("key");

            var signedXml = new SignedXml(xmlDoc) {SigningKey = key};

            var reference = new Reference { Uri = "#presented-form" };

            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            //signedXml.ComputeSignature();
            //var xmlDigitalSignature = signedXml.GetXml();
            //xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            //xmlDoc.Save("out-signed-xml.xml");
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
            request.AddParameter("Authorization", (new N3Credentials(Constants.TestToken)).ToString(), ParameterType.HttpHeader);
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
            request.AddParameter("Authorization", (new N3Credentials(Constants.TestToken)).ToString(), ParameterType.HttpHeader);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.AddParameter("_format", "json", ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            return (Parameters)FhirParser.ParseFromJson(response.Content);
        }

        public static Resource GetOrderResponse(Order order, DiagnosticReport[] reports)
        {
            return new OrderResponse
            {
                Identifier = new List<Identifier> { new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS)) },
                Request = FhirHelper.CreateReference(order),
                Date = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Who = order.Source,
                OrderStatus_ = OrderResponse.OrderStatus.Completed,
                Description = "Комментарий к заказу",
                Fulfillment = reports.Select(FhirHelper.CreateBundleReference).ToList()
            };
        }

        public static Practitioner GetPractitioner(ResourceReference orderOrganization)
        {
            var practitioner = new Practitioner
            {
                Identifier = new List<Identifier>
                {
                    new Identifier(Systems.ORGANIZATIONS, GetCodeBySystem(Systems.ORGANIZATIONS))
                },
                Id = Guid.NewGuid().ToString(),
                Name = new HumanName
                {
                    Family = new List<string> { "Петров" },
                    Given = new List<string> { "Петр" }
                },
                PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
                {
                    new Practitioner.PractitionerPractitionerRoleComponent
                    {
                        ManagingOrganization = orderOrganization,
                        Role = new CodeableConcept(Systems.PRACTITIONER_ROLE, GetCodeBySystem(Systems.PRACTITIONER_ROLE)),
                        Specialty = new List<CodeableConcept>
                        {
                            new CodeableConcept(Systems.PRACTITIONER_SPECIALITY, GetCodeBySystem(Systems.PRACTITIONER_SPECIALITY))
                        }
                    }
                }
            };

            return practitioner;
        }
    }
}
