using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Suyeong.Core.DB.MsSql
{
    public static class MsSqlDB
    {
        public static string GetDbConStr(string id, string password, string serverIP, string databaseName)
        {
            return $"Persist Security Info=False;User ID={id};Password={password};Server={serverIP};Database={databaseName};";
        }

        public static object GetDataSingle(string conStr, string query, SqlParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query, SqlParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
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

        public static DataTable GetDataTable(string conStr, string query, SqlParameter[] parameters = null)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter())
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, SqlParameter[] parameters = null)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters));
        }

        public static DataSet GetDataSet(string conStr, string query, SqlParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter())
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, SqlParameter[] parameters = null)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters));
        }

        public static bool SetQuery(string conStr, string query, SqlParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
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

        public static bool SetQuery(string conStr, DataTable table)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = table.TableName;

                        foreach (DataColumn column in table.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        bulkCopy.WriteToServer(table);
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, SqlParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
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

        async public static Task<bool> SetQueryAsync(string conStr, DataTable table)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = table.TableName;

                        foreach (DataColumn column in table.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        await bulkCopy.WriteToServerAsync(table);
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
