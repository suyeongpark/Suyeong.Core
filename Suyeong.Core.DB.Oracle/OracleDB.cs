using System;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace Suyeong.Core.DB.Oracle
{
    public static class OracleDB
    {
        public static string GetDbConStr(string id, string password, string dataSource)
        {
            return $"Persist Security Info=False;User ID={id};Password={password};Data Source={dataSource};Min Pool Size=10;Max Pool Size=50;Connection Lifetime=120;";
        }

        public static object GetDataSingle(string conStr, string query, OracleParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        scalar = command.ExecuteScalar();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query, OracleParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        scalar = await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return scalar;
        }

        public static DataTable GetDataTable(string conStr, string query, OracleParameter[] parameters = null)
        {
            DataTable table = new DataTable();

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (OracleDataAdapter adapter = new OracleDataAdapter())
                        {
                            adapter.SelectCommand = command;
                            adapter.Fill(table);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return table;
        }

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, OracleParameter[] parameters = null)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters));
        }

        public static DataSet GetDataSet(string conStr, string query, OracleParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (OracleDataAdapter adapter = new OracleDataAdapter())
                        {
                            adapter.SelectCommand = command;
                            adapter.Fill(dataSet);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dataSet;
        }

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, OracleParameter[] parameters = null)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters));
        }

        public static bool SetQuery(string conStr, string query, OracleParameter[] parameters = null, int bindCount = 0)
        {
            bool result = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        if (bindCount > 0)
                        {
                            command.ArrayBindCount = bindCount;
                        }

                        command.ExecuteNonQuery();
                    }
                }

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, OracleParameter[] parameters = null, int bindCount = 0)
        {
            bool result = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {

                        if (parameters != null)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        if (bindCount > 0)
                        {
                            command.ArrayBindCount = bindCount;
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }

                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
