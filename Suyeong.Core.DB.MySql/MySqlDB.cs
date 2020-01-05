using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Suyeong.Core.DB.MySql
{
    public static class MySqlDB
    {
        // mysql bulk insert는 아래 링크 참조.
        // https://dev.mysql.com/doc/refman/5.7/en/insert.html
        // Oracle이나 MsSql과 달리 별도의 파라미터가 있지는 않고 그냥 values를 여러 번 넘긴다.
        // insert into TABLE_NAME (a,b,c) values (1,2,3),(4,5,6),(7,8,9);

        // mysql bulk update는 아래 참조
        // https://www.tutorialspoint.com/how-to-bulk-update-mysql-data-with-a-single-query
        // update UpdateAllDemo
        //−> set BookName = (CASE BookId WHEN 1000 THEN 'C in Depth'
        //−> when 1001 THEN 'Java in Depth'
        //−> END)
        //−> Where BookId IN (1000,1001);

        public static string GetDbConStr(string serverIP, string databaseName, string uid, string password)
        {
            return $"Server={serverIP};Database={databaseName};Uid={uid};Pwd={password};";
        }

        public static object GetDataSingle(string conStr, string query)
        {
            object scalar = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        scalar = command.ExecuteScalar();
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        public static object GetDataSingle(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = command.ExecuteScalar();
                    }
                    catch (MySqlException)
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

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return scalar;
        }

        async public static Task<object> GetDataSingleAsync(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            object scalar = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        scalar = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    }
                    catch (MySqlException)
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

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static DataTable GetDataTable(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataTable table = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query)
        {
            DataTable table = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        await adapter.FillAsync(table).ConfigureAwait(false);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataTable table = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        await adapter.FillAsync(table).ConfigureAwait(false);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static DataSet GetDataSet(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        public static DataSet GetDataSet(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataSet dataSet = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query)
        {
            DataSet dataSet = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        adapter.SelectCommand = command;
                        await adapter.FillAsync(dataSet).ConfigureAwait(false);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            DataSet dataSet = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter())
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        adapter.SelectCommand = command;
                        await adapter.FillAsync(dataSet).ConfigureAwait(false);
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                }
            }

            return dataSet;
        }

        public static bool SetQuery(string conStr, string query)
        {
            int result = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (MySqlException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public static bool SetQuery(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (MySqlException)
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

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (MySqlException)
                    {
                        command.Transaction.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        async public static Task<bool> SetQueryAsync(string conStr, string query, MySqlParameter[] parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException();
            }

            int result = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString: conStr))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                using (MySqlCommand command = new MySqlCommand(cmdText: query, connection: connection, transaction: transaction))
                {
                    try
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (result > 0)
                        {
                            command.Transaction.Commit();
                        }
                    }
                    catch (MySqlException)
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
