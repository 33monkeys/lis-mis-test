using System.Collections.Generic;

namespace Lis.Test.Integration.Terminology
{
    public interface IPostgresTerm
    {
        bool CreateDictionary(string dictionaryName, string system);
        bool DeleteDictionary(string system);

        bool IsIerarchicalDictionary(string system, out string parentId);

        TerminologyDictionaryItem Read(string system, string code, string version);
        bool Create(string system, string code, string version, string display, Dictionary<string, string> content);
        long Delete(string system, string code, string version);

        List<TerminologyDictionaryItem> Search(string system, string version, string filter);
        List<TerminologyDictionaryItem> Search(string system, string filter);
        bool Validate(string system, string code, string version);
    }
}
