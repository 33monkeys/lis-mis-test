using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Monads.NET;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace Lis.Test.Integration.Terminology
{
    public class PostgresHelper
    {
        public static object Func(string funcName, params NpgsqlParameter[] parameters)
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();

            try
            {
                var command = new NpgsqlCommand(funcName, conn) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddRange(parameters);
                var result = command.ExecuteScalar();
                return result;
            }
            catch (Exception ex)
            {
                throw new PostresException(
                    string.Format("Call {0} terminology function failed. Reason {1}", funcName, ex.Message),
                    ex);
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<TerminologyDictionaryItem> Read(string name, NpgsqlParameter[] parameters)
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();

            try
            {
                var command = new NpgsqlCommand(name, conn) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddRange(parameters);
                var dataReader = command.ExecuteReader();

                var result = new List<TerminologyDictionaryItem>();
                while (dataReader.Read())
                {
                    var item = new TerminologyDictionaryItem();

                    dataReader
                        .If(x => x.HasOrdinal("content"))
                        .With(x => x["content"])
                        .Do(x =>
                        {
                            item.Content = JsonConvert.DeserializeObject<Dictionary<string, string>>(x.ToString());
                        });

                    dataReader
                        .If(x => x.HasOrdinal("code"))
                        .With(x => x["code"])
                        .Do(x =>
                        {
                            item.Code = x.ToString();
                        });

                    dataReader
                        .If(x => x.HasOrdinal("display"))
                        .With(x => x["display"])
                        .Do(x =>
                        {
                            item.Display = x.ToString();
                        });

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new PostresException(
                    string.Format("Call {0} terminology function failed. Reason {1}", name, ex.Message),
                    ex);
            }
            finally
            {
                conn.Close();
            }
        }

        public static object ExecuteCommand(NpgsqlCommand command)
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();

            try
            {
                command.Connection = conn;
                var result = command.ExecuteScalar();
                return result;
            }
            catch (Exception ex)
            {
                throw new PostresException(string.Format("Call {0} terminology command failed. Reason {1}", command, ex.Message), ex);
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<string> ExecuteCommandReader(NpgsqlCommand command)
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();

            try
            {
                command.Connection = conn;
                var reader = command.ExecuteReader();
                var result = new List<string>();
                while (reader.Read())
                    result.Add(reader[0].ToString());

                return result;
            }
            catch (Exception ex)
            {
                throw new PostresException(string.Format("Call {0} terminology command failed. Reason {1}", command, ex.Message), ex);
            }
            finally
            {
                conn.Close();
            }
        }

        private static string GetConnectionString()
        {
            if (ConfigurationManager.ConnectionStrings["TerminologyConnection"] == null)
                throw new PostresException("Add \"TerminologyConnection\" connection string in app.config or web.config");

            return ConfigurationManager.ConnectionStrings["TerminologyConnection"].ConnectionString;
        }

        public static NpgsqlParameter Text(string text)
        {
            return new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Text,
                Value = text,
            };
        }

        public static NpgsqlParameter Jsonb(string text)
        {
            return new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                Value = text,
            };
        }

        public static NpgsqlParameter TextArray(string[] resources)
        {
            return new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Array,
                Value = resources,
            };
        }

        public static NpgsqlParameter Int(int limit)
        {
            return new NpgsqlParameter
            {
                NpgsqlDbType = NpgsqlDbType.Integer,
                Value = limit,
            };
        }
    }
}
