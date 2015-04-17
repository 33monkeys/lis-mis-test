//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Hl7.Fhir.Model;
//using Hl7.Fhir.Rest;
//using Hl7.Fhir.Serialization;
//using Lis.Test.Integration.Common;
//using NUnit.Framework;
//using RestSharp;

//namespace Lis.Test
//{
//    partial class LisOrderScenarios
//    {
//        //private const string Endpoint = "http://fhir.zdrav.netrika.ru/fhir";
//        private const string Endpoint = "http://localhost:50883/fhir";

//        public FhirClient Client { get; private set; }

//        //[TestFixtureSetUp]
//        public void Init()
//        {
//            Client = new FhirClient(Endpoint) {PreferredFormat = ResourceFormat.Json, UseFormatParam = true,};
//            ClearTables();
//        }

//        [Test]
//        [Category("Invalid order")]
//        [ExpectedException]
//        [Description("Бандл без Order.Subject")]
//        public void FullLisOrder_Invalid_Order_Subject_Transaction()
//        {
//            var lisOrderBundle = CreateLisOrderBundle();

//            //Поломаем ссылки в бандле
//            var order = (Order)lisOrderBundle.Entry.First(x => x.Resource is Order).Resource;
//            order.Subject = null;

//            var result = Client.Transaction(lisOrderBundle);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Entry, Is.Not.Null);
//            Assert.That(result.Entry.Count, Is.GreaterThan(0));
//        }

//        [Test]
//        [Category("Invalid order")]
//        [ExpectedException]
//        [Description("Бандл без Order.Source")]
//        public void FullLisOrder_Invalid_Order_Source_Transaction()
//        {
//            var lisOrderBundle = CreateLisOrderBundle();

//            //Поломаем ссылки в бандле
//            var order = (Order)lisOrderBundle.Entry.First(x => x.Resource is Order).Resource;
//            order.Source = null;

//            var result = Client.Transaction(lisOrderBundle);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Entry, Is.Not.Null);
//            Assert.That(result.Entry.Count, Is.GreaterThan(0));
//        }

//        [Test]
//        [Category("Invalid order")]
//        [ExpectedException]
//        [Description("Бандл без DiagnosticOrder.Subject")]
//        public void FullLisOrder_Invalid_DiagnosticOrder_Subject_Transaction()
//        {
//            var lisOrderBundle = CreateLisOrderBundle();

//            //Поломаем ссылки в бандле
//            var order = (DiagnosticOrder)lisOrderBundle.Entry.First(x => x.Resource is DiagnosticOrder).Resource;
//            order.Subject = null;

//            var result = Client.Transaction(lisOrderBundle);

//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.Entry, Is.Not.Null);
//            Assert.That(result.Entry.Count, Is.GreaterThan(0));
//        }

//        [Test]
//        [Description("Сценарий, c неверными ссылками в полном бандле (пока не работает)")]
//        public void FullLisOrder_Full_Order_Broken_References_Transaction()
//        {
//            Assert.Inconclusive();
//        }

//        #region Helpers

//        public List<Resource> ValidResources
//        {
//            get
//            {
//                #region Dirty

//                var patient = LoadResource<Patient>("Patient.json");
//                patient.Id = Guid.NewGuid().ToString();

//                var orderPractitioner = LoadResource<Practitioner>("Practitioner.json");
//                orderPractitioner.Id = Guid.NewGuid().ToString();

//                var diagnosticOrderPractitioner = LoadResource<Practitioner>("Practitioner-1.json");
//                diagnosticOrderPractitioner.Id = Guid.NewGuid().ToString();

//                var specimen = LoadResource<Specimen>("Specimen.json");
//                specimen.Id = Guid.NewGuid().ToString();
//                specimen.Subject = FhirHelper.CreateReference(patient);

//                var specimen1 = LoadResource<Specimen>("Specimen-1.json");
//                specimen1.Id = Guid.NewGuid().ToString();
//                specimen1.Subject = FhirHelper.CreateReference(patient);

//                var condition = LoadResource<Condition>("Condition.json");
//                condition.Id = Guid.NewGuid().ToString();
//                condition.Patient = FhirHelper.CreateReference(patient);

//                var encounter = LoadResource<Encounter>("Encounter.json");
//                encounter.Id = Guid.NewGuid().ToString();
//                encounter.Patient = FhirHelper.CreateReference(patient);
//                encounter.Indication = new List<ResourceReference> {FhirHelper.CreateReference(condition)};


//                var orderOrganization = LoadResource<Organization>("Organization.json");
//                orderOrganization.Id = Guid.NewGuid().ToString();

//                var diagnosticOrderOrganization = LoadResource<Organization>("Organization-1.json");
//                diagnosticOrderOrganization.Id = Guid.NewGuid().ToString();

//                var observation = LoadResource<Observation>("Observation.json");
//                observation.Id = Guid.NewGuid().ToString();

//                var observation1 = LoadResource<Observation>("Observation-1.json");
//                observation1.Id = Guid.NewGuid().ToString();

//                orderPractitioner.PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
//                {
//                    new Practitioner.PractitionerPractitionerRoleComponent
//                    {
//                        ManagingOrganization = FhirHelper.CreateReference(orderOrganization)
//                    }
//                };

//                diagnosticOrderPractitioner.PractitionerRole = new List
//                    <Practitioner.PractitionerPractitionerRoleComponent>
//                {
//                    new Practitioner.PractitionerPractitionerRoleComponent
//                    {
//                        ManagingOrganization = FhirHelper.CreateReference(diagnosticOrderOrganization)
//                    }
//                };

//                #endregion

//                return new List<Resource>
//                {
//                    patient,
//                    orderPractitioner,
//                    diagnosticOrderPractitioner,
//                    specimen,
//                    specimen1,
//                    condition,
//                    encounter,
//                    orderOrganization,
//                    diagnosticOrderOrganization,
//                    observation,
//                    observation1
//                };
//            }
//        }

//        private Bundle CreateLisOrderBundle()
//        {
//            var patient = Client.Create(LoadResource<Patient>("Patient.json"));
//            var orderPractitioner = Client.Create(LoadResource<Practitioner>("Practitioner.json"));
//            var diagnosticOrderPractitioner = Client.Create(LoadResource<Practitioner>("Practitioner-1.json"));

//            var sm = LoadResource<Specimen>("Specimen.json");
//            var sm1 = LoadResource<Specimen>("Specimen-1.json");

//            sm.Subject = FhirHelper.CreateReference(patient);
//            sm1.Subject = FhirHelper.CreateReference(patient);

//            var specimen = Client.Create(sm);
//            var specimen1 = Client.Create(sm1);

//            var cnd = LoadResource<Condition>("Condition.json");
//            cnd.Patient = FhirHelper.CreateReference(patient);
//            var condition = Client.Create(cnd);

//            var enc = LoadResource<Encounter>("Encounter.json");
//            enc.Patient = FhirHelper.CreateReference(patient);
//            enc.Indication = new List<ResourceReference> {FhirHelper.CreateReference(condition) };
//            var encounter = Client.Create(enc);


//            var orderOrganization = Client.Create(LoadResource<Organization>("Organization.json"));
//            var organization1 = Client.Create(LoadResource<Organization>("Organization-1.json"));
//            var observation = Client.Create(LoadResource<Observation>("Observation.json"));
//            var observation1 = Client.Create(LoadResource<Observation>("Observation-1.json"));

//            orderPractitioner.PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
//            {
//                new Practitioner.PractitionerPractitionerRoleComponent
//                {
//                    ManagingOrganization = FhirHelper.CreateReference(orderOrganization)
//                }
//            };

//            diagnosticOrderPractitioner.PractitionerRole = new List<Practitioner.PractitionerPractitionerRoleComponent>
//            {
//                new Practitioner.PractitionerPractitionerRoleComponent
//                {
//                    ManagingOrganization = FhirHelper.CreateReference(organization1)
//                }
//            };

//            var diagnosticOrder = LoadResource<DiagnosticOrder>("DiagnosticOrder.json");
//            diagnosticOrder.Id = "45BFD414-E9E6-4D00-A785-FE5FFEF74B5D";
//            diagnosticOrder.Subject = FhirHelper.CreateReference(patient);
//            diagnosticOrder.Orderer = FhirHelper.CreateReference(diagnosticOrderPractitioner);
//            diagnosticOrder.Encounter = FhirHelper.CreateReference(encounter);
//            diagnosticOrder.SupportingInformation = new List<ResourceReference>
//            {
//                FhirHelper.CreateReference(observation), FhirHelper.CreateReference(observation1),
//            };
//            diagnosticOrder.Specimen = new List<ResourceReference>
//            {
//                FhirHelper.CreateReference(specimen), FhirHelper.CreateReference(specimen1)
//            };

//            //Услуги 
//            diagnosticOrder.Item = new List<DiagnosticOrder.DiagnosticOrderItemComponent>
//            {
//                new DiagnosticOrder.DiagnosticOrderItemComponent{Code = new CodeableConcept
//                {
//                    Text = "Услуга 1",
//                    Coding = new List<Coding>
//                    {
//                        new Coding("http://hl7.org", "услуга1"),
//                    }
//                }},
//                new DiagnosticOrder.DiagnosticOrderItemComponent{Code = new CodeableConcept
//                {
//                    Text = "Услуга 2",
//                    Coding = new List<Coding>
//                    {
//                        new Coding("http://hl7.org", "услуга2"),
//                    }
//                }},
//                new DiagnosticOrder.DiagnosticOrderItemComponent{Code = new CodeableConcept
//                {
//                    Text = "Услуга 3",
//                    Coding = new List<Coding>
//                    {
//                        new Coding("http://hl7.org", "услуга3"),
//                    }
//                }}
//            };
            
//            var order = LoadResource<Order>("Order.json");
//            order.Subject = FhirHelper.CreateReference(patient);
//            order.Source = FhirHelper.CreateReference(orderPractitioner);
//            order.Target = FhirHelper.CreateReference(orderOrganization);
//            order.Detail = new List<ResourceReference>{new ResourceReference{Reference = diagnosticOrder.Id}};

//            var lisOrderBundle = new Bundle
//            {
//                Meta = new Meta
//                {
//                    Profile = new List<string> { GetLisOrderDtructureDefinition().Reference}, 
//                },
//                Type = Bundle.BundleType.Transaction,
//                Entry = new List<Bundle.BundleEntryComponent>
//                {
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = diagnosticOrder,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = diagnosticOrder.TypeName,
//                        }
//                    },
//                    new Bundle.BundleEntryComponent
//                    {
//                        Resource = order,
//                        Transaction = new Bundle.BundleEntryTransactionComponent
//                        {
//                            Method = Bundle.HTTPVerb.POST,
//                            Url = order.TypeName
//                        }
//                    }
//                }
//            };

//            return lisOrderBundle;
//        }

//        static T LoadResource<T>(string name) where T : Base
//        {
//            var bundleJson = File.ReadAllText(Path.Combine("Iteration-2/Data/Resources", name));
//            return (T)null;
//            //return (T) FhirParser.ParseFromJson(bundleJson);
//        }

//        static ResourceReference GetLisOrderDtructureDefinition()
//        {
//            var reference = string.Format("StructureDefinition/LisOrder");
//            return new ResourceReference
//            {
//                Reference = reference
//            };
//        }

//        static Bundle LoadTransactionBundle(string path)
//        {
//            var bundleJson = File.ReadAllText(Path.Combine("Iteration-2/Data", path));
//            return (Bundle)FhirParser.ParseFromJson(bundleJson);
//        }

//        private void ClearTables()
//        {
//            ClearTable<Condition>();
//            ClearTable<DiagnosticOrder>();
//            ClearTable<Encounter>();
//            ClearTable<Observation>();
//            ClearTable<Order>();
//            ClearTable<Organization>();
//            ClearTable<Patient>();
//            ClearTable<Practitioner>();
//            ClearTable<Specimen>();
//        }

//        void ClearTable<T>() where T : Resource, new()
//        {
//            var response = Client.Search<T>();
//            foreach (var component in response.Entry)
//                Client.Delete(component.Resource);
//        }

//        #endregion
//    }
//}
