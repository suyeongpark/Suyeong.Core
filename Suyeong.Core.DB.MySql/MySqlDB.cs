using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Suyeong.Core.DB.MySql
{
    public static class MySqlDB
    {
        // mysql bulk insert는 아래 링크 참조.
        // https://dev.mysql.com/doc/refman/5.7/en/insert.html
        // Oracle이나 MsSql과 달리 별도의 파라미터가 있지는 않고 그냥 values를 여러 번 다넘긴.
        // INSERT INTO tbl_name (a,b,c) VALUES(1,2,3),(4,5,6),(7,8,9);

        public static string GetDbConStr(string serverIP, string databaseName, string uid, string password)
        {
            return $"Server={serverIP};Database={databaseName};Uid={uid};Pwd={password};";
        }

        public static object GetDataSingle(string conStr, string query, MySqlParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query, MySqlParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {

                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
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

        public static DataTable GetDataTable(string conStr, string query, MySqlParameter[] parameters = null)
        {
            DataTable table = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {

                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter())
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, MySqlParameter[] parameters = null)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters));
        }

        public static DataSet GetDataSet(string conStr, string query, MySqlParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter())
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, MySqlParameter[] parameters = null)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters));
        }

        public static bool SetQuery(string conStr, string query, MySqlParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, MySqlParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
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
    }
}
