using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Monads.NET;
using NUnit.Framework;

namespace Lis.Test.Utility
{
    [TestFixture]
    class Class1
    {
        [Test]
        public void Test()
        {
            var parameters = new Parameters();
            "source-code".Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "SourceCode",
                Value = new FhirString(x)
            }));

            "target-code".Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "TargetCode",
                Value = new FhirString(x)
            }));

            "barcode".Do(x => parameters.Parameter.Add(new Parameters.ParametersParameterComponent
            {
                Name = "Barcode",
                Value = new FhirString(x)
            }));

            var jsonToSend = FhirSerializer.SerializeToJson(parameters);
            File.WriteAllText("out.json", jsonToSend);
        }
    }
}
