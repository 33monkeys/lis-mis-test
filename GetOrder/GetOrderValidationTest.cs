using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Lis.Test.Integration.Common;
using NUnit.Framework;
using RestSharp;

namespace Lis.Test.GetOrder
{
    [TestFixture]
    class GetOrderValidationTest
    {
        [TestCaseSource("ValidParameters")]
        public void Test_Valid_GetOrder_Parameters(Parameters parameters)
        {
            var result = GetOrderOperation(parameters);
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));
        }

        [TestCaseSource("InvalidParameters")]
        public void Test_Invalid_GetOrder_Parameters(Parameters parameters)
        {
            var result = GetOrderOperation(parameters);
            Assert.That(result, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCaseSource("ValidParameters")]
        public void TestFhirParser(Parameters parameters)
        {
            var json = FhirSerializer.SerializeToJson(parameters);
            var parsed = (Parameters)FhirParser.ParseResourceFromJson(json);

            Assert.That(parameters.IsExactly(parsed));
        }

        public List<Parameters> ValidParameters
        {
            get
            {
                return new List<Parameters>
                {
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "SourceCode", Value = new FhirString("123456")},
                            new Parameters.ParametersParameterComponent{Name = "TargetCode", Value = new FhirString("654321")},
                            new Parameters.ParametersParameterComponent{Name = "Barcode", Value = new FhirString("111-222-333")},
                        }
                    },
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "TargetCode", Value = new FhirString("654321")},
                            new Parameters.ParametersParameterComponent{Name = "Barcode", Value = new FhirString("111-222-333")},
                        }
                    }
                };
            }
        }

        public List<Parameters> InvalidParameters
        {
            get
            {
                return new List<Parameters>
                {
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "SourceCode", Value = new FhirString("123456")},
                            new Parameters.ParametersParameterComponent{Name = "TargetCode", Value = new FhirString("654321")},
                        }
                    },
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "SourceCode", Value = new FhirString("123456")},
                            new Parameters.ParametersParameterComponent{Name = "Barcode", Value = new FhirString("111-222-333")},
                        }
                    },
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "SourceCode", Value = new FhirString("123456")},
                        }
                    },
                    new Parameters
                    {
                        Parameter = new List<Parameters.ParametersParameterComponent>
                        {
                            new Parameters.ParametersParameterComponent{Name = "SourceCode", Value = new FhirString("123456")},
                            new Parameters.ParametersParameterComponent{Name = "TargetCode", Value = new FhirString("654321")},
                            new Parameters.ParametersParameterComponent{Name = "TargetCode1", Value = new FhirString("654321")},
                            new Parameters.ParametersParameterComponent{Name = "Barcode", Value = new FhirString("111-222-333")},
                        }
                    },
                };
            }
        }

        public static HttpStatusCode GetOrderOperation(Parameters parameters)
        {
            var client = new RestClient(Constants.Endpoint);
            var request = new RestRequest("$getorder", Method.POST) {RequestFormat = DataFormat.Json};
            request.AddHeader("Accept", "application/json");

            var jsonToSend = FhirSerializer.SerializeToJson(parameters);
            request.Parameters.Clear();
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            //request.AddParameter("_format", "json");
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            return response.StatusCode;
        }
    }
}
