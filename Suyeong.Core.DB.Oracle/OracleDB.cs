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

        public static object GetDataSingle(string conStr, string query)
        {
            object scalar = null;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        scalar = command.ExecuteScalar();
                    }
                    catch (OracleException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        public static object GetDataSingle(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = command.ExecuteScalar();
                    }
                    catch (OracleException)
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

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (OracleException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (OracleException)
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

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (OracleException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static DataTable GetDataTable(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataTable table = new DataTable();

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter())
                {
                    try
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (OracleException)
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, OracleParameter[] parameters)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (OracleException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        public static DataSet GetDataSet(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataSet dataSet = new DataSet();

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter())
                {
                    try
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (OracleException)
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, OracleParameter[] parameters)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static bool SetQuery(string conStr, string query)
        {
            int result = 0;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction())
                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        command.Transaction = transaction;

                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (OracleException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction())
                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        command.Transaction = transaction;

                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (OracleException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, OracleParameter[] parameters, int bindCount)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction())
                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        command.Transaction = transaction;

                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        command.ArrayBindCount = bindCount;

                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (OracleException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query)
        {
            int result = 0;

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleTransaction transaction = connection.BeginTransaction())
                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    {
                        try
                        {
                            command.Transaction = transaction;

                            result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                            if (result > 0)
                            {
                                command.Transaction.Commit();
                            }
                        }
                        catch (OracleException)
                        {
                            command.Transaction.Rollback();
                            throw;
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, OracleParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction())
                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        command.Transaction = transaction;

                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (OracleException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, OracleParameter[] parameters, int bindCount)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (OracleConnection connection = new OracleConnection(connectionString: conStr))
            {
                connection.Open();

                using (OracleTransaction transaction = connection.BeginTransaction())
                using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                {
                    try
                    {
                        command.Transaction = transaction;

                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        command.ArrayBindCount = bindCount;

                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (OracleException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }
    }
}
