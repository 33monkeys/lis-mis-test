using Hl7.Fhir.Model;

namespace Lis.Test.Integration.Common
{
    class FhirHelper
    {
        public static ResourceReference CreateReference(Resource resource)
        {
            var reference = string.Format("{0}/{1}", resource.TypeName, resource.Id);
            return new ResourceReference
            {
                Reference = reference
            };
        }
    }
}