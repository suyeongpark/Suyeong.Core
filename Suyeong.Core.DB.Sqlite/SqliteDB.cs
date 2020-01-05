using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Suyeong.Core.DB.Sqlite
{
    public static class SqliteDB
    {
        public static string GetDbConStr(string dataSource, string password)
        {
            return $"Data Source={dataSource};Version=3;Pooling=True;Max Pool Size=100;Password={password};";
        }

        public static object GetDataSingle(string conStr, string query)
        {
            object scalar = null;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                {
                    try
                    {
                        scalar = command.ExecuteScalar();
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        public static object GetDataSingle(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = command.ExecuteScalar();
                    }
                    catch (SQLiteException)
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

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                {
                    try
                    {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (SQLiteException)
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

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static DataTable GetDataTable(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataTable table = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (SQLiteException)
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, SQLiteParameter[] parameters)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (SQLiteException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        public static DataSet GetDataSet(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataSet dataSet = new DataSet();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (SQLiteException)
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, SQLiteParameter[] parameters)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters)).ConfigureAwait(false);
        }

        public static bool SetQuery(string conStr, string query)
        {
            int result = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SQLiteException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SQLiteException)
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

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SQLiteException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, SQLiteParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString: conStr))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                using (SQLiteCommand command = new SQLiteCommand(commandText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        foreach (SQLiteParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (SQLiteException)
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
