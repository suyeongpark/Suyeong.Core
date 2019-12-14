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

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
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

        public static object GetDataSingle(string conStr, string query, OracleParameter[] parameters)
        {
            object scalar = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    {
                        foreach (OracleParameter parameter in parameters)
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
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query, OracleParameter[] parameters)
        {
            object scalar = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    {
                        foreach (OracleParameter parameter in parameters)
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
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    using (OracleDataAdapter adapter = new OracleDataAdapter())
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

        public static DataTable GetDataTable(string conStr, string query, OracleParameter[] parameters)
        {
            DataTable table = new DataTable();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    using (OracleDataAdapter adapter = new OracleDataAdapter())
                    {
                        foreach (OracleParameter parameter in parameters)
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, OracleParameter[] parameters = null)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters));
        }

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    using (OracleDataAdapter adapter = new OracleDataAdapter())
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

        public static DataSet GetDataSet(string conStr, string query, OracleParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString: conStr))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(cmdText: query, conn: connection))
                    using (OracleDataAdapter adapter = new OracleDataAdapter())
                    {
                        foreach (OracleParameter parameter in parameters)
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, OracleParameter[] parameters = null)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters));
        }

        public static bool SetQuery(string conStr, string query)
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
                        command.Transaction = transaction;

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
                            catch (OracleException)
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

        public static bool SetQuery(string conStr, string query, OracleParameter[] parameters)
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
                        command.Transaction = transaction;

                        try
                        {
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
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (OracleException)
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

        public static bool SetQuery(string conStr, string query, OracleParameter[] parameters, int bindCount)
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
                        command.Transaction = transaction;

                        try
                        {
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
                        catch (Exception)
                        {
                            try
                            {
                                command.Transaction.Rollback();
                            }
                            catch (OracleException)
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
                        command.Transaction = transaction;

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
                            catch (OracleException)
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, OracleParameter[] parameters)
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
                        command.Transaction = transaction;

                        try
                        {
                            foreach (OracleParameter parameter in parameters)
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
                            catch (OracleException)
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, OracleParameter[] parameters, int bindCount)
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
                        command.Transaction = transaction;

                        try
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }

                            command.ArrayBindCount = bindCount;

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
                            catch (OracleException)
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
    }
}
