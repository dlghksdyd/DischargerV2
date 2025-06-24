using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Sqlite.Server
{
    public static class SqliteUserInfo
    {
        private static readonly string ClassName = "dbo.MST_USER_INFO";

        private static string ConnectionString = "Data Source=127.0.0.1,1433;Initial Catalog=MINDIMS;User ID=sa;Password=mintech1234;";

        public static void UpdateConnectionString(string serverIp, string serverPort, string databaseName)
        {
            ConnectionString = $@"Data Source={serverIp},{serverPort};Initial Catalog={databaseName};User ID=sa;Password=mintech1234;";
        }

        public static TABLE_MST_USER_INFO FindUserInfo(string userId, string password)
        {
            var userInfoList = GetData();

            return userInfoList.Find(x => x.USER_ID == userId && x.PASSWD == password);
        }

        public static List<TABLE_MST_USER_INFO> GetData()
        {
            List<TABLE_MST_USER_INFO> table = new List<TABLE_MST_USER_INFO>();

            using (SqliteConnection connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TABLE_MST_USER_INFO tableMstUserInfo = new TABLE_MST_USER_INFO
                        {
                            USER_ID = reader["USER_ID"].ToString(),
                            USER_NM = reader["USER_NM"].ToString(),
                            PASSWD = reader["PASSWD"].ToString(),
                            ADMIN_GROUP = reader["ADMIN_GROUP"].ToString(),
                            USE_YN = reader["USE_YN"].ToString(),
                        };

                        table.Add(tableMstUserInfo);
                    }
                }
            }

            return table;
        }
    }
}
