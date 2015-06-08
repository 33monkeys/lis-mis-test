using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Lis.Test.Integration.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Lis.Test.Terminology
{
    class TerminologyHelper
    {
        private readonly Lazy<RestClient> _lazyTermClient = new Lazy<RestClient>(() => new RestClient(Constants.TerminologyEndpoint));
        private static readonly Lazy<RestClient> LazyDictClient = new Lazy<RestClient>(() => new RestClient(Constants.DictionaryEndpoint));

        public RestClient TermClient { get { return _lazyTermClient.Value; } }

        public static RestClient DictClient { get { return LazyDictClient.Value; } }


        public static IRestResponse CreateDictionary(string name, string system)
        {
            var request = new RestRequest("$dictionary", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("name", name, ParameterType.QueryString);
            request.AddParameter("system", system, ParameterType.QueryString);

            var result = DictClient.Execute(request);
            return result;
        }

        public static IRestResponse DeleteDictionary(string system)
        {
            var request = new RestRequest("$dictionary", Method.DELETE);
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("system", system, ParameterType.QueryString);

            var result = DictClient.Execute(request);
            return result;
        }

        public static IRestResponse CreateItem(string system, string code, string display, string json)
        {
            var request = new RestRequest("$item", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.AddParameter("system", system, ParameterType.QueryString);
            request.AddParameter("code", code, ParameterType.QueryString);
            request.AddParameter("display", display, ParameterType.QueryString);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var result = DictClient.Execute(request);
            return result;
        }

        public static void LoadMkb10()
        {
            using (var reader = new StreamReader("Terminology/mkb10.csv", Encoding.GetEncoding("windows-1251")))
            using (var parser = new TextFieldParser(reader))
            {
                parser.Delimiters = new[] {";"};
                parser.TextFieldType = FieldType.Delimited;

                var names = parser.ReadFields();
                var taked = 0;
                while (!parser.EndOfData && taked < 500)
                {
                    var values = parser.ReadFields();
                    var json = CreateJsonDict(names, values);

                    CreateItem(Constants.Mkb10DictionarySystem, values[1], values[3], json);
                    taked++;
                }
            }
        }

        private static string CreateJsonDict(string[] names, string[] values)
        {
            var dict = names.Zip(values, (s, s1) => new Tuple<string, string>(s, s1))
                .ToDictionary(k => k.Item1, v => v.Item2);
            var result = new Parameters();
            foreach (var item in dict)
                result.Add(item.Key, new FhirString(item.Value));

            return FhirSerializer.SerializeResourceToJson(result);
        }

        public static void DeleteMkb10()
        {
            var deleteResponse = DeleteDictionary(Constants.Mkb10DictionarySystem);
            //Assert.That(deleteResponse, Is.Not.Null);
            //Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public static void CreateMkb10()
        {
            var response = CreateDictionary(Constants.Mkb10DictionaryName, 
                Constants.Mkb10DictionarySystem);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public static void CreateMkb10ValueSet()
        {
            //throw new NotImplementedException();
        }
    }
}
