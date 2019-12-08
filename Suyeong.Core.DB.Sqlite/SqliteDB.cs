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

        public static object GetDataSingle(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            object scalar = null;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
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

        async public static Task<object> GetDataSingleAsync(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            object scalar = null;

            try
            {

                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {

                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
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

        public static DataTable GetDataTable(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            DataTable table = new DataTable();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
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

        async public static Task<DataTable> GetDataTableAsync(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            return await Task.Run<DataTable>(() => GetDataTable(conStr: conStr, query: query, parameters: parameters));
        }

        public static DataSet GetDataSet(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter())
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

        async public static Task<DataSet> GetDataSetAsync(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            return await Task.Run<DataSet>(() => GetDataSet(conStr: conStr, query: query, parameters: parameters));
        }

        public static bool SetQuery(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
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

        async public static Task<bool> SetQueryAsync(string conStr, string query, SQLiteParameter[] parameters = null)
        {
            bool result = false;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(conStr))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (SQLiteParameter parameter in parameters)
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
