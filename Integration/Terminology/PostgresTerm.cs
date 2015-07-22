using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Lis.Test.Integration.Terminology
{
    public class PostgresTerm : IPostgresTerm
    {
        public bool CreateDictionary(string dictionaryName, string system)
        {
            return PostgresTerminology
                .Call("dict.fx_create_dictionary")
                .WithText(dictionaryName)
                .WithText(system)
                .Cast<bool>();
        }

        public bool DeleteDictionary(string system)
        {
            return PostgresTerminology
                .Call("dict.fx_delete_dictionary")
                .WithText(system)
                .Cast<bool>();
        }

        public TerminologyDictionaryItem Read(string system, string code, string version)
        {
            var json = PostgresTerminology
                .Call("dict.fx_read")
                .WithText(system)
                .WithText(code)
                .WithText(version)
                .Read();

            return json.FirstOrDefault();
        }

        public bool Create(string system, string code, string version, string display, Dictionary<string, string> content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);

            return PostgresTerminology
                .Call("dict.fx_create")
                .WithText(system)
                .WithText(code)
                .WithText(display)
                .WithJsonb(jsonContent)
                .WithText(version)
                .Cast<bool>();
        }

        public long Delete(string system, string code, string version)
        {
            return PostgresTerminology
                .Call("dict.fx_delete")
                .WithText(system)
                .WithText(code)
                .WithText(version)
                .Cast<long>();
        }

        public List<TerminologyDictionaryItem> Search(string system, string version, string filter)
        {
            var dataReader = PostgresTerminology
                .Call("dict.fx_search")
                .WithText(system)
                .WithText(filter ?? string.Empty)
                .WithText(version)
                .Read();

            return dataReader;
        }

        public List<TerminologyDictionaryItem> Search(string system, string filter)
        {
            var dataReader = PostgresTerminology
                .Call("dict.fx_search")
                .WithText(system)
                .WithText(filter ?? string.Empty)
                .Read();

            return dataReader;
        }

        public bool Validate(string system, string code, string version)
        {
            return PostgresTerminology
                .Call("dict.fx_validate")
                .WithText(system)
                .WithText(code)
                .WithText(version)
                .Cast<bool>();
        }

        public bool IsIerarchicalDictionary(string system, out string parentId)
        {
            parentId = PostgresTerminology.GetDictionaryParentId(system);
            return !string.IsNullOrEmpty(parentId);
        }
    }
}