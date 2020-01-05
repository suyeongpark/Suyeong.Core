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

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        scalar = command.ExecuteScalar();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        public static object GetDataSingle(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = command.ExecuteScalar();
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query)
        {
            object scalar = null;

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        public static DataTable GetDataTable(string conStr, string query)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static DataTable GetDataTable(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    try
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query)).ConfigureAwait(false);
        }

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, SqlParameter[] parameters)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        public static DataSet GetDataSet(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataSet dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    try
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query)).ConfigureAwait(false);
        }

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, SqlParameter[] parameters)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static bool SetQuery(string conStr, string query)
        {
            int result = 0;

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
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

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
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, DataTable table)
        {
            if (table == null)
            {
                throw new NullReferenceException();
            }
            else if (string.IsNullOrWhiteSpace(table.TableName))
            {
                throw new ArgumentNullException(table.TableName);
            }

            bool result = false;

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
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand command = new SqlCommand(cmdText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

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

                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, DataTable table)
        {
            if (table == null)
            {
                throw new NullReferenceException();
            }
            else if (string.IsNullOrWhiteSpace(table.TableName))
            {
                throw new ArgumentNullException(table.TableName);
            }

            bool result = false;

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

                        await bulkCopy.WriteToServerAsync(table).ConfigureAwait(false);
                        transaction.Commit();
                        result = true;
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return result;
        }
    }
}
