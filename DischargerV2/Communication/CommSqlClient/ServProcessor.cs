using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Xml.Linq;

namespace SqlClient.Server
{
    public class SqlClient
    {
        public static string ConnectionString = "Data Source=127.0.0.1,1433;Initial Catalog=MINDIMS;User ID=sa;Password=mintech1234;";

        public static void UpdateConnectionString(string serverIp, string serverPort, string databaseName)
        {
            ConnectionString = $@"Data Source={serverIp},{serverPort};Initial Catalog={databaseName};User ID=sa;Password=mintech1234;";
        }
    }

    public static class SqlClientUserInfo 
    {
        private static readonly string ClassName = "dbo.MST_USER_INFO";

        public static TABLE_MST_USER_INFO FindUserInfo(string userId, string password)
        {
            var userInfoList = GetData();

            return userInfoList.Find(x => x.USER_ID == userId && x.PASSWD == password);
        }

        public static List<TABLE_MST_USER_INFO> GetData()
        {
            List<TABLE_MST_USER_INFO> table = new List<TABLE_MST_USER_INFO>();

            using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
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

    public static class SqlClientStatus
    {
        private static readonly string ClassName = "dbo.SYS_STS_SDC";

        public static bool InsertData_Init(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $@"
                            IF EXISTS (SELECT * FROM {ClassName} WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}')
                                BEGIN
                                    UPDATE {ClassName} SET
                                    MC_NM='{data.MC_NM}',
                                    MC_DTM=GETDATE()
                                    WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}'
                                END 
                            ELSE 
                                BEGIN
                                   INSERT INTO {ClassName}
                                   (MC_CD,MC_CH,MC_NM,MC_DTM)
                                   VALUES('{data.MC_CD}','{data.MC_CH}','{data.MC_NM}',GETDATE())
                                END";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateData(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"UPDATE {ClassName} SET " +
                        $"USER_NM='{data.USER_NM}'," +
                        $"DischargerVoltage='{data.DischargerVoltage}'," +
                        $"DischargerCurrent='{data.DischargerCurrent}'," +
                        $"DischargerTemp='{data.DischargerTemp}'," +
                        $"MC_DTM=GETDATE() " +
                        $"WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateData_StateNTime(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"UPDATE {ClassName} SET " +
                        $"USER_NM='{data.USER_NM}'," +
                        $"DischargerState='{data.DischargerState}'," +
                        $"ProgressTime='{data.ProgressTime}'," +
                        $"MC_DTM=GETDATE() " +
                        $"WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateData_Set(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"UPDATE {ClassName} SET " +
                        $"USER_NM='{data.USER_NM}'," +
                        $"DischargeMode='{data.DischargeMode}'," +
                        $"DischargeTarget='{data.DischargeTarget}'," +
                        $"LogFileName='{data.LogFileName}'," +
                        $"MC_DTM=GETDATE() " +
                        $"WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
