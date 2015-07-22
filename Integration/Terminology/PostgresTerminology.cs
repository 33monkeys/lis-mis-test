using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace Lis.Test.Integration.Terminology
{
    public static class PostgresTerminology
    {
        /// <summary>
        /// Create Posgres function
        /// </summary>
        /// <param name="fhirbaseFunc"></param>
        /// <returns></returns>
        public static PostgresFunc Call(string fhirbaseFunc)
        {
            return new PostgresFunc { Name = fhirbaseFunc };
        }

        /// <summary>
        /// Add text parameter
        /// </summary>
        /// <param name="func"></param>
        /// <param name="textParameter"></param>
        /// <returns></returns>
        public static PostgresFunc WithText(this PostgresFunc func, string textParameter)
        {
            func.Parameters.Add(PostgresHelper.Text(textParameter));
            return func;
        }

        /// <summary>
        /// Add jsonb parameter
        /// </summary>
        /// <param name="func"></param>
        /// <param name="jsonParameter"></param>
        /// <returns></returns>
        public static PostgresFunc WithJsonb(this PostgresFunc func, string jsonParameter)
        {
            func.Parameters.Add(PostgresHelper.Jsonb(jsonParameter));
            return func;
        }

        public static PostgresFunc WithTextArray(this PostgresFunc func, string[] resources)
        {
            func.Parameters.Add(PostgresHelper.TextArray(resources));
            return func;
        }

        public static PostgresFunc WithInt(this PostgresFunc func, int limit)
        {
            func.Parameters.Add(PostgresHelper.Int(limit));
            return func;
        }

        /// <summary>
        /// Call Postgres function and cast value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Cast<T>(this PostgresFunc func)
        {
            var result = PostgresHelper.Func(func.Name, func.Parameters.ToArray());
            return result is DBNull ? default(T) : (T)result;
        }

        public static List<TerminologyDictionaryItem> Read(this PostgresFunc func)
        {
            var result = PostgresHelper.Read(func.Name, func.Parameters.ToArray());
            return result;
        }

        public static string GetDictionaryParentId(string system)
        {
            var request = new NpgsqlCommand("SELECT parent_id_name FROM dict.dictionary WHERE system_uri=@URI;");
            request.Parameters.AddWithValue("@URI", NpgsqlDbType.Text, system);

            var result = PostgresHelper.ExecuteCommand(request);
            return result.ToString();
        }

        public class PostgresFunc
        {
            public PostgresFunc()
            {
                Parameters = new List<NpgsqlParameter>();
            }

            public string Name { get; set; }

            public List<NpgsqlParameter> Parameters { get; set; }
        }
    }
}
