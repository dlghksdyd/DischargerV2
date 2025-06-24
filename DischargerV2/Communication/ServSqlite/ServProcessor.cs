using System.Collections.Generic;
using System.Data.SqlClient;

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

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TABLE_MST_USER_INFO tableMstUserInfo = new TABLE_MST_USER_INFO
                        {
                            USER_ID = reader["USER_ID"].ToString().Trim(),
                            USER_NM = reader["USER_NM"].ToString().Trim(),
                            PASSWD = reader["PASSWD"].ToString().Trim(),
                            ADMIN_GROUP = reader["ADMIN_GROUP"].ToString().Trim(),
                            USE_YN = reader["USE_YN"].ToString().Trim(),
                        };

                        table.Add(tableMstUserInfo);
                    }
                }
            }

            return table;
        }
    }
}
