using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

namespace SqlClient.Server
{
    public class SqlClient
    {
        public static string ConnectionString = "Data Source=192.168.20.7,1433;Initial Catalog=MINDIMS_MOE;User ID=mindims;Password=mindims;";
        //public static string ConnectionString = "Data Source=127.0.0.1,1433;Initial Catalog=MINDIMS_MOE;User ID=sa;Password=mintech1234;";

        public static void UpdateConnectionString(string serverIp, string serverPort, string databaseName)
        {
            ConnectionString = $@"Data Source={serverIp},{serverPort};Initial Catalog={databaseName};User ID=mindims;Password=mindims;";
            //ConnectionString = $@"Data Source={serverIp},{serverPort};Initial Catalog={databaseName};User ID=sa;Password=mintech1234;";
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
                                    DischargerName='{data.DischargerName}',
                                    MC_DTM=GETDATE()
                                    WHERE MC_CD='{data.MC_CD}' AND MC_CH='{data.MC_CH}'
                                END 
                            ELSE 
                                BEGIN
                                   INSERT INTO {ClassName}
                                   (MC_CD,MC_CH,DischargerName,MC_DTM)
                                   VALUES('{data.MC_CD}','{data.MC_CH}','{data.DischargerName}',GETDATE())
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

        public static bool UpdateData_Monitoring(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"UPDATE {ClassName} SET " +
                        $"USER_ID='{data.USER_ID}'," +
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
                        $"USER_ID='{data.USER_ID}'," +
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
                        $"USER_ID='{data.USER_ID}'," +
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

        public static bool UpdateData_Discharging(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"UPDATE {ClassName} SET " +
                        $"USER_ID='{data.USER_ID}'," +
                        $"DischargeCapacity_Ah='{data.DischargeCapacity_Ah}'," +
                        $"DischargeCapacity_kWh='{data.DischargeCapacity_kWh}'," +
                        $"DischargePhase='{data.DischargePhase}'," +
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

        public static bool UpdateData_Alarm(TABLE_QLT_HISTORY_ALARM data)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.IFT_HISTORY_ALARM";

                        command.Parameters.Add("@MTYPE", SqlDbType.VarChar, 50);
                        command.Parameters.Add("@MC_CD", SqlDbType.VarChar, 7);
                        command.Parameters.Add("@CH_NO", SqlDbType.Int);
                        command.Parameters.Add("@Alarm_Time", SqlDbType.DateTime2);
                        command.Parameters.Add("@Alarm_Code", SqlDbType.Int);
                        command.Parameters.Add("@Alarm_Desc", SqlDbType.VarChar, 80);

                        command.Parameters["@MTYPE"].Value = "SDC";
                        command.Parameters["@MC_CD"].Value = data.MC_CD;
                        command.Parameters["@CH_NO"].Value = data.CH_NO;
                        command.Parameters["@Alarm_Time"].Value = data.Alarm_Time;
                        command.Parameters["@Alarm_Code"].Value = data.Alarm_Code;
                        command.Parameters["@Alarm_Desc"].Value = data.Alarm_Desc;

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

        public static bool DeleteData(string machineCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlClient.ConnectionString))
                {
                    connection.Open();

                    string query = $"DELETE {ClassName} " +
                        $"WHERE MC_CD='{machineCode}'";

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
