using System.Collections.Generic;

namespace Lis.Test.Integration.Terminology
{
    public class TerminologyDictionaryItem
    {
        public string Code { get; set; }

        public string Display { get; set; }

        public string Version { get; set; }

        public Dictionary<string, string> Content { get; set; }
    }
}