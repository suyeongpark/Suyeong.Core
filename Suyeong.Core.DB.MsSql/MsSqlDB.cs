using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Suyeong.Core.DB.MsSql
{
    public static class MsSqlDB
    {
        public static string GetDbConStr(string id, string password, string serverName, string databaseName)
        {
            return $"Persist Security Info=False;User ID={id};Password={password};Server={serverName};Database={databaseName};";
        }

        public static object GetDataSingle(string conStr, string query)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    {
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

        public static object GetDataSingle(string conStr, string query, SqlParameter[] parameters)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    {
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query, SqlParameter[] parameters)
        {
            object scalar = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
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

        public static DataTable GetDataTable(string conStr, string query)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return table;
        }

        public static DataTable GetDataTable(string conStr, string query, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
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

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dataSet;
        }

        public static DataSet GetDataSet(string conStr, string query, SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
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

        public static bool SetQuery(string conStr, string query)
        {
            int result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection, transaction: transaction))
                    {
                        try
                        {
                            result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                command.Transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, SqlParameter[] parameters)
        {
            int result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection, transaction: transaction))
                    {
                        try
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }

                            result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                command.Transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, DataTable table)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection: connection, copyOptions: SqlBulkCopyOptions.Default, externalTransaction: transaction))
                    {
                        try
                        {
                            bulkCopy.DestinationTableName = table.TableName;

                            foreach (DataColumn column in table.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                            }

                            bulkCopy.WriteToServer(table);
                            transaction.Commit();
                            result = true;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query)
        {
            int result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection, transaction: transaction))
                    {
                        try
                        {
                            result = await command.ExecuteNonQueryAsync();

                            if (result > 0)
                            {
                                command.Transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, SqlParameter[] parameters)
        {
            int result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection, transaction: transaction))
                    {
                        try
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }

                            result = await command.ExecuteNonQueryAsync();

                            if (result > 0)
                            {
                                command.Transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, DataTable table)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection: connection, copyOptions: SqlBulkCopyOptions.Default, externalTransaction: transaction))
                    {
                        try
                        {
                            bulkCopy.DestinationTableName = table.TableName;

                            foreach (DataColumn column in table.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                            }

                            await bulkCopy.WriteToServerAsync(table);
                            transaction.Commit();
                            result = true;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (SqlException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
