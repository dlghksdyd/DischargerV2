using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Sqlite.Common
{
    public static class SqliteUtility
    {
        public static void InitializeDatabases()
        {
            /// Database 디렉토리가 없으면 생성
            DirectoryInfo di = new DirectoryInfo("Database");
            if (!di.Exists)
            {
                di.Create();
            }

            /// 각 테이블이 없으면 생성
            SqliteUserInfo.CreateTable();
            SqliteDischargerModel.CreateTable();
            SqliteDischargerInfo.CreateTable();
            SqliteDischargerErrorCode.CreateTable();
            SqliteDischargerErrorCode.InitializeData();

            /// 어드민 계정이 없으면 생성
            List<TableUserInfo> userAdmin = SqliteUserInfo.GetData().FindAll(x => x.UserId == "admin");
            if (userAdmin.Count == 0)
            {
                TableUserInfo userInsert = new TableUserInfo
                {
                    UserId = "admin",
                    Password = "1",
                    UserName = "admin",
                    IsAdmin = true,
                };
                SqliteUserInfo.InsertData(userInsert);
            }
        }
    }

    public static class SqliteUserInfo
    {
        private static readonly string ClassName = nameof(SqliteUserInfo);

        private static readonly string ConnectionString = @"Data Source=|DataDirectory|\Database\" + ClassName + ".db";

        public static void CreateTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"CREATE TABLE IF NOT EXISTS " + ClassName + "(";
                query += "'UserId' TEXT PRIMARY KEY, ";
                query += "'Password' TEXT, ";
                query += "'UserName' TEXT, ";
                query += "'IsAdmin' BOOL";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool InsertData(TableUserInfo oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"INSERT INTO " + ClassName + " (";
                    query += "'UserId',";
                    query += "'Password',";
                    query += "'UserName',";
                    query += "'IsAdmin'";
                    query += ") ";
                    query += "values (";
                    query += "'" + oneRowData.UserId + "', ";
                    query += "'" + oneRowData.Password + "', ";
                    query += "'" + oneRowData.UserName + "', ";
                    query += "'" + oneRowData.IsAdmin + "'";
                    query += ")";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static List<TableUserInfo> GetData()
        {
            List<TableUserInfo> table = new List<TableUserInfo>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableUserInfo oneRow = new TableUserInfo();
                        oneRow.UserId = reader["UserId"].ToString();
                        oneRow.Password = reader["Password"].ToString();
                        oneRow.UserName = reader["UserName"].ToString();
                        oneRow.IsAdmin = Boolean.Parse(reader["IsAdmin"].ToString());
                        table.Add(oneRow);
                    }
                }
            }

            return table;
        }

        public static bool UpdateData(TableUserInfo oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    var query = "";
                    query = @"UPDATE " + ClassName + " SET ";
                    query += "\"Password\"='" + oneRowData.Password.ToString() + "',";
                    query += "\"UserName\"='" + oneRowData.UserName.ToString() + "'";
                    query += "WHERE \"UserId\"='" + oneRowData.UserId + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static bool DeleteData(string userId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"DELETE FROM " + ClassName + " WHERE \"UserId\"=='" + userId + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

    public static class SqliteDischargerModel
    {
        private static readonly string ClassName = nameof(SqliteDischargerModel);

        private static readonly string ConnectionString = @"Data Source=|DataDirectory|\Database\" + ClassName + ".db";

        public static void CreateTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"CREATE TABLE IF NOT EXISTS " + ClassName + "(";
                query += "'Id' INTEGER NOT NULL, ";
                query += "'Model' TEXT, ";
                query += "'Type' TEXT, ";
                query += "'Channel' INTEGER, ";
                query += "'SpecVoltage' REAL, ";
                query += "'SpecCurrent' REAL, ";
                query += "'SafetyVoltMax' REAL, ";
                query += "'SafetyVoltMin' REAL, ";
                query += "'SafetyCurrentMax' REAL, ";
                query += "'SafetyCurrentMin' REAL, ";
                query += "'SafetyTempMax' REAL, ";
                query += "'SafetyTempMin' REAL,";
                query += "PRIMARY KEY('Id' AUTOINCREMENT)";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool InsertData(TableDischargerModel oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"INSERT INTO " + ClassName + " (";
                    query += "'Id',";
                    query += "'Model',";
                    query += "'Type',";
                    query += "'Channel',";
                    query += "'SpecVoltage',";
                    query += "'SpecCurrent',";
                    query += "'SafetyVoltMax',";
                    query += "'SafetyVoltMin',";
                    query += "'SafetyCurrentMax',";
                    query += "'SafetyCurrentMin',";
                    query += "'SafetyTempMax',";
                    query += "'SafetyTempMin'";
                    query += ") ";
                    query += "values (";
                    query += "'" + oneRowData.Id.ToString() + "', ";
                    query += "'" + oneRowData.Model.ToString() + "', ";
                    query += "'" + oneRowData.Type.ToString() + "', ";
                    query += "'" + oneRowData.Channel + "', ";
                    query += "'" + oneRowData.SpecVoltage + "', ";
                    query += "'" + oneRowData.SpecCurrent + "', ";
                    query += "'" + oneRowData.SafetyVoltMax + "', ";
                    query += "'" + oneRowData.SafetyVoltMin + "', ";
                    query += "'" + oneRowData.SafetyCurrentMax + "', ";
                    query += "'" + oneRowData.SafetyCurrentMin + "', ";
                    query += "'" + oneRowData.SafetyTempMax + "', ";
                    query += "'" + oneRowData.SafetyTempMin + "'";
                    query += ")";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static List<TableDischargerModel> GetData()
        {
            List<TableDischargerModel> table = new List<TableDischargerModel>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableDischargerModel oneRow = new TableDischargerModel();
                        oneRow.Id = int.Parse(reader["Id"].ToString());
                        oneRow.Model = (EDischargerModel)Enum.Parse(typeof(EDischargerModel), reader["Model"].ToString());
                        oneRow.Type = (EDischargeType)Enum.Parse(typeof(EDischargeType), reader["Type"].ToString());
                        oneRow.Channel = int.Parse(reader["Channel"].ToString());
                        oneRow.SpecVoltage = double.Parse(reader["SpecVoltage"].ToString());
                        oneRow.SpecCurrent = double.Parse(reader["SpecCurrent"].ToString());
                        oneRow.SafetyVoltMax = double.Parse(reader["SafetyVoltMax"].ToString());
                        oneRow.SafetyVoltMin = double.Parse(reader["SafetyVoltMin"].ToString());
                        oneRow.SafetyCurrentMax = double.Parse(reader["SafetyCurrentMax"].ToString());
                        oneRow.SafetyCurrentMin = double.Parse(reader["SafetyCurrentMin"].ToString());
                        oneRow.SafetyTempMax = double.Parse(reader["SafetyTempMax"].ToString());
                        oneRow.SafetyTempMin = double.Parse(reader["SafetyTempMin"].ToString());

                        table.Add(oneRow);
                    }
                }
            }

            return table;
        }

        public static bool UpdateData(TableDischargerModel oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    var query = "";
                    query = @"UPDATE " + ClassName + " SET ";
                    query += "\"Model\"='" + oneRowData.Model.ToString() + "',";
                    query += "\"Type\"='" + oneRowData.Type.ToString() + "',";
                    query += "\"Channel\"='" + oneRowData.Channel.ToString() + "',";
                    query += "\"SpecVoltage\"='" + oneRowData.SpecVoltage.ToString() + "',";
                    query += "\"SpecCurrent\"='" + oneRowData.SpecCurrent.ToString() + "',";
                    query += "\"SafetyVoltMax\"='" + oneRowData.SafetyVoltMax.ToString() + "',";
                    query += "\"SafetyVoltMin\"='" + oneRowData.SafetyVoltMin.ToString() + "',";
                    query += "\"SafetyCurrentMax\"='" + oneRowData.SafetyCurrentMax.ToString() + "',";
                    query += "\"SafetyCurrentMin\"='" + oneRowData.SafetyCurrentMin.ToString() + "',";
                    query += "\"SafetyTempMax\"='" + oneRowData.SafetyTempMax.ToString() + "',";
                    query += "\"SafetyTempMin\"='" + oneRowData.SafetyTempMin.ToString() + "'";
                    query += "WHERE \"Id\"='" + oneRowData.Id + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static bool DeleteData(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"DELETE FROM " + ClassName + " WHERE \"Id\"=='" + id + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

    public static class SqliteDischargerInfo
    {
        private static readonly string ClassName = nameof(SqliteDischargerInfo);

        private static readonly string ConnectionString = @"Data Source=|DataDirectory|\Database\" + ClassName + ".db";

        public static void CreateTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"CREATE TABLE IF NOT EXISTS " + ClassName + "(";
                query += "'Name' TEXT PRIMARY KEY, ";
                query += "'Model' TEXT, ";
                query += "'Type' TEXT, ";
                query += "'Channel' INTEGER, ";
                query += "'SpecVoltage' REAL, ";
                query += "'SpecCurrent' REAL, ";
                query += "'IpAddress' TEXT, ";
                query += "'IsTempModule' INTEGER, ";
                query += "'TempModuleComPort' TEXT, ";
                query += "'TempModuleChannel' TEXT, ";
                query += "'TempChannel' TEXT";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool UpdateData(TableDischargerInfo oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    var query = "";
                    query = @"UPDATE " + ClassName + " SET ";
                    query += "\"Model\"='" + oneRowData.Model.ToString() + "',";
                    query += "\"Type\"='" + oneRowData.Type.ToString() + "',";
                    query += "\"Channel\"='" + oneRowData.DischargerChannel + "',";
                    query += "\"SpecVoltage\"='" + oneRowData.SpecVoltage + "',";
                    query += "\"SpecCurrent\"='" + oneRowData.SpecCurrent + "',";
                    query += "\"IpAddress\"='" + oneRowData.IpAddress + "',";
                    query += "\"IsTempModule\"='" + (oneRowData.IsTempModule == true ? 1 : 0) + "',";
                    query += "\"TempModuleComPort\"='" + oneRowData.TempModuleComPort + "',";
                    query += "\"TempModuleChannel\"='" + oneRowData.TempModuleChannel + "', ";
                    query += "\"TempChannel\"='" + oneRowData.TempChannel + "'";
                    query += "WHERE \"Name\"='" + oneRowData.DischargerName + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static bool InsertData(TableDischargerInfo oneRowData)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"INSERT INTO " + ClassName + " (";
                    query += "'Name',";
                    query += "'Model',";
                    query += "'Type',";
                    query += "'Channel',";
                    query += "'SpecVoltage',";
                    query += "'SpecCurrent',";
                    query += "'IpAddress',";
                    query += "'IsTempModule',";
                    query += "'TempModuleComPort',";
                    query += "'TempModuleChannel',";
                    query += "'TempChannel'";
                    query += ") ";
                    query += "values (";
                    query += "'" + oneRowData.DischargerName + "', ";
                    query += "'" + oneRowData.Model.ToString() + "', ";
                    query += "'" + oneRowData.Type.ToString() + "', ";
                    query += "'" + oneRowData.DischargerChannel + "', ";
                    query += "'" + oneRowData.SpecVoltage + "', ";
                    query += "'" + oneRowData.SpecCurrent + "', ";
                    query += "'" + oneRowData.IpAddress + "', ";
                    query += "'" + (oneRowData.IsTempModule == true ? 1 : 0) + "', ";
                    query += "'" + oneRowData.TempModuleComPort + "', ";
                    query += "'" + oneRowData.TempModuleChannel + "', ";
                    query += "'" + oneRowData.TempChannel + "'";
                    query += ")";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

        public static List<TableDischargerInfo> GetData()
        {
            List<TableDischargerInfo> table = new List<TableDischargerInfo>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableDischargerInfo oneRow = new TableDischargerInfo();
                        oneRow.DischargerName = reader["Name"].ToString();
                        oneRow.Model = (EDischargerModel)Enum.Parse(typeof(EDischargerModel), reader["Model"].ToString());
                        oneRow.Type = (EDischargeType)Enum.Parse(typeof(EDischargeType), reader["Type"].ToString());
                        oneRow.DischargerChannel = short.Parse(reader["Channel"].ToString());
                        oneRow.SpecVoltage = double.Parse(reader["SpecVoltage"].ToString());
                        oneRow.SpecCurrent = double.Parse(reader["SpecCurrent"].ToString());
                        oneRow.IpAddress = reader["IpAddress"].ToString();
                        oneRow.IsTempModule = reader["IsTempModule"].ToString() == "1" ? true : false;
                        oneRow.TempModuleComPort = reader["TempModuleComPort"].ToString();
                        oneRow.TempModuleChannel = reader["TempModuleChannel"].ToString();
                        oneRow.TempChannel = reader["TempChannel"].ToString();
                        table.Add(oneRow);
                    }
                }
            }

            return table;
        }

        public static TableDischargerInfo GetData(string dischargerName)
        {
            TableDischargerInfo table = new TableDischargerInfo();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;
                query += " WHERE Name = '" + dischargerName + "'";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        table.DischargerName = reader["Name"].ToString();
                        table.Model = (EDischargerModel)Enum.Parse(typeof(EDischargerModel), reader["Model"].ToString());
                        table.Type = (EDischargeType)Enum.Parse(typeof(EDischargeType), reader["Type"].ToString());
                        table.DischargerChannel = short.Parse(reader["Channel"].ToString());
                        table.SpecVoltage = double.Parse(reader["SpecVoltage"].ToString());
                        table.SpecCurrent = double.Parse(reader["SpecCurrent"].ToString());
                        table.IpAddress = reader["IpAddress"].ToString();
                        table.IsTempModule = reader["IsTempModule"].ToString() == "1" ? true : false;
                        table.TempModuleComPort = reader["TempModuleComPort"].ToString();
                        table.TempModuleChannel = reader["TempModuleChannel"].ToString();
                        table.TempChannel = reader["TempChannel"].ToString();
                    }
                }
            }

            return table;
        }

        public static bool DeleteData(string name)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "";
                    query += @"DELETE FROM " + ClassName + " WHERE \"Name\"=='" + name + "'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
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

    public static class SqliteDischargerErrorCode
    {
        private static readonly string ClassName = nameof(SqliteDischargerErrorCode);

        private static readonly string ConnectionString = @"Data Source=|DataDirectory|\Database\" + ClassName + ".db";

        public static void CreateTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                var instance = new TableDischargerErrorCode();

                string query = @"CREATE TABLE IF NOT EXISTS " + ClassName + "(";
                query += "'" + nameof(instance.Code) + "' INTEGER PRIMARY KEY NOT NULL, ";
                query += "'" + nameof(instance.Name) + "' TEXT, ";
                query += "'" + nameof(instance.Title) + "' TEXT, ";
                query += "'" + nameof(instance.Description) + "' TEXT, ";
                query += "'" + nameof(instance.Cause) + "' TEXT, ";
                query += "'" + nameof(instance.Action) + "' TEXT";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InitializeData()
        {
            /// 모든 데이터 삭제
            DeleteData();

            TableDischargerErrorCode oneRow;

            oneRow = new TableDischargerErrorCode(0x01001000, "ERR_HW_INPUT_REV", "Cable reverse connection Alarm", "Cable 역결선", "전류케이블 극성이 반대입니다.", "전류케이블 극성을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01001100, "ERR_HW_INPUT_CABLE", "Cable connection Alarm", "Cable 결선 오류", "결선을 확인하십시오.", "전류CABLE 또는, 배터리 센싱선 결선을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01001200, "ERR_HW_SYNC_REV", "SYNC current reverse connection Alarm", "SYNC전류센서 역결선", "SYNC 전류값의 방향이 반대입니다.", "SYNC 전류센서 극성을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01002000, "ERR_HW_COMM_ID", "ID Setting Alarm", "ID 설정 오류", "ID 설정오류 입니다.", "SINGLE운전 : CAN_ID = 0번, SYNC운전 : CAN_ID = 1~15번");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01002100, "ERR_HW_COMM_LINK", "UI Communication Alarm", "모니터링 통신불량 (PC-파워모듈)", "통신이 끊어 졌습니다.", "전원확인 및 통신라인을 점검하고, ID를 점검 하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01002200, "ERR_HW_COMM_PARAL", "Parallel Communication Alarm", "모듈간 통신불량 (파워모듈 -파워모듈)", "모듈 통신이 끊어졌습니다", "모듈의 후면 패널에서 통신 케이블 연결및 ID설정을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01003000, "ERR_HW_OT_IGBT1", "Heatsink Over Temperature Alarm 1", "방열판 과열 1", "모듈의 방열판 과열", "FAN상태를 확인하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01003100, "ERR_HW_OT_IGBT2", "Heatsink Over Temperature Alarm 2", "방열판 과열 2", "모듈의 방열판 과열", "FAN상태를 확인하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x05003000, "ERR_HW_OT_LOAD1", "Load Heatsink Over Temperature Alarm 1", "부하 저항 과열 1", "모듈의 방열판 과열", "FAN상태를 확인하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01004000, "ERR_HW_FAN1_IGBT1", "IGBT 1 Fan Alarm 1", "모듈 1 FAN 고장 1", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01004100, "ERR_HW_FAN2_IGBT1", "IGBT 1 Fan Alarm 2", "모듈 1 FAN 고장 2", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01004200, "ERR_HW_FAN3_IGBT1", "IGBT 1 Fan Alarm 3", "모듈 1 FAN 고장 3", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01004300, "ERR_HW_FAN4_IGBT1", "IGBT 1 Fan Alarm 4", "모듈 1 FAN 고장 4", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01004400, "ERR_HW_FAN1_IGBT2", "IGBT 2 Fan Alarm 1", "모듈 2 FAN 고장 1", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x05004000, "ERR_HW_FAN1_LOAD1", "Load Fan Alarm 1", "부하 FAN 고장", "FAN고장입니다.", "전원을 확인하십시오. 문제가있는 FAN을 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005000, "ERR_HW_IGBT", "IGTB Driver Alarm", "스위칭 소자 고장", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005100, "ERR_HW_IGBT1", "IGTB Driver Alarm 1", "스위칭 소자 고장 1", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005200, "ERR_HW_IGBT2", "IGTB Driver Alarm 2", "스위칭 소자 고장 2", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005300, "ERR_HW_IGBT3", "IGTB Driver Alarm 3", "스위칭 소자 고장 3", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005400, "ERR_HW_IGBT4", "IGTB Driver Alarm 4", "스위칭 소자 고장 4", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01005500, "ERR_HW_IGBT5", "IGTB Driver Alarm 5", "스위칭 소자 고장 5", "모듈의 IGBT Driver Error가 발생하였습니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x05005000, "ERR_HW_LOAD1", "Load Value Alarm", "부하 저항값 이상 2", "부하저항 모듈 고장입니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01006000, "ERR_HW_RLY1", "Relay operation Alarm 1", "릴레이 동작 고장 1", "릴레이 고장입니다.", "전원을 확인하십시오. 문제가 있는 릴레이를 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01006100, "ERR_HW_RLY2", "Relay operation Alarm 2", "릴레이 동작 고장 2", "릴레이 고장입니다.", "전원을 확인하십시오. 문제가 있는 릴레이를 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01006200, "ERR_HW_RLY3", "Relay operation Alarm 3", "릴레이 동작 고장 3", "릴레이 고장입니다.", "전원을 확인하십시오. 문제가 있는 릴레이를 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01006300, "ERR_HW_RLY4", "Relay operation Alarm 4", "릴레이 동작 고장 4", "릴레이 고장입니다.", "전원을 확인하십시오. 문제가 있는 릴레이를 교체하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007000, "ERR_HW_EMG", "EMG Switch ON Alarm", "접접류 고장 EMG", "비상 스위치가 켜져 있습니다.", "EMG 버튼을 해제하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007100, "ERR_HW_FUSE1", "Fuse Alarm 1", "접접류 고장 Fuse 1", "Fuse 고장입니다.", "문제가 있는 Fuse를 교체하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007200, "ERR_HW_FUSE2", "Fuse Alarm 2", "접접류 고장 Fuse 2", "Fuse 고장입니다.", "문제가 있는 Fuse를 교체하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007A00, "ERR_HW_SPD1", "SPD Alarm 1", "접접류 고장 SPD 1", "SPD 고장입니다.", "문제가 있는 SPD를 교체하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007B00, "ERR_HW_SPD2", "SPD Alarm 2", "접접류 고장 SPD 2", "SPD 고장입니다.", "문제가 있는 SPD를 교체하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007C00, "ERR_HW_ACPWR", "Input AC Power Alarm", "접접류 AC 전원 이상", "한전 전원 이상", "한전 전원을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01007D00, "ERR_HW_SWRUN", "RUN Switch Operation Alarm", "접접류 RUN 스위치 이상", "주전원 입력 시 또는 고장 초기화시 RUN 스위치 조작 이상입니다.", "RUN 스위치 OFF후 주전원 입력 또는 고장 초기화를 하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01008000, "ERR_HW_SMPS_5V", "SMPS Power 5V Alarm", "SMPS 5V 저전압", "5V 전원 장치 고장입니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x05008000, "ERR_HW_SMPS_12V", "SMPS Power 12V Alarm", "SMPS 12V 저전압", "12V 전원 장치 고장입니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01009000, "ERR_HW_SLAVE", "추후 정리", "DCDC Module Slave 이상", "", "");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100, "ERR_HW_", "추후 정리", "Spare", "", "");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100F100, "ERR_HW_OV_INPUT", "Over Voltage Sensing Alarm", "ACDC 측 과전압 (Latch)", "센싱 전압이  설정치 보다 높습니다", "전원 상태 및 연결 케이블을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100F200, "ERR_HW_UV_INPUT", "Under Voltage Sensing Alarm", "ACDC측 저전압 (Latch)", "센싱 전압이  설정치 보다 낮습니다", "전원 상태 및 연결 케이블을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100F300, "ERR_HW_OC_CHGBATT", "Charge Battery Over Current Alarm", "배터리 충전 OCP (Latch)", "배터리 전류가 제한을 초과합니다.", "전류 및 전력 제한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0X0100F400, "ERR_HW_OC_DISCBATT", "Discharge Battery Over Current Alarm", "배터리 방전 OCP (Latch)", "배터리 전류가 제한을 초과합니다.", "전류 및 전력 제한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100F500, "ERR_HW_OC_IGBT1", "IGBT Over Current  Sensing Alarm 1", "IGBT 과전류 1", "모듈의 IGBT에 동작전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0X0100F600, "ERR_HW_OC_IGBT2", "IGBT Over Current  Sensing Alarm 2", "IGBT 과전류 2", "모듈의 IGBT에 동작전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000010, "ERR_SW_OV_BATT", "Battery Over Voltage Alarm", "BATTERY 과전압", "배터리 전압이 상한을 초과합니다.", "전압 상한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000011, "ERR_SW_OV_INPUT1", "Over Voltage Sensing Alarm 1", "입력 과전압 1", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000012, "ERR_SW_OV_OUTPUT1", "Output voltage upper limit Alarm 1", "출력 과전압 1", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000013, "ERR_SW_OV_INPUT2", "Over Voltage Sensing Alarm 2", "입력 과전압 2", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000014, "ERR_SW_OV_OUTPUT2", "Output voltage upper limit Alarm 2", "출력 과전압 2", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000015, "ERR_SW_OV_INPUT3", "Over Voltage Sensing Alarm 3", "입력 과전압 3", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000016, "ERR_SW_OV_OUTPUT3", "Output voltage upper limit Alarm 3", "출력 과전압 3", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000017, "ERR_SW_OV_INPUT4", "Over Voltage Sensing Alarm 4", "입력 과전압 4", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000018, "ERR_SW_OV_OUTPUT4", "Output voltage upper limit Alarm 4", "출력 과전압 4 ", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000019, "ERR_SW_OV_INPUT5", "Over Voltage Sensing Alarm 5", "입력 과전압 5", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100001A, "ERR_SW_OV_OUTPUT5", "Output voltage upper limit Alarm 5", "출력 과전압 5", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100001B, "ERR_SW_OV_INPUT6", "Over Voltage Sensing Alarm 6", "입력 과전압 6", "센싱 전압이  설정치 보다 높습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x0100001C, "ERR_SW_OV_OUTPUT6", "Output voltage upper limit Alarm 6", "출력 과전압 6", "출력전압이 기준치이상으로 초과합니다.", "배터리 센싱전압 LINE을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000020, "ERR_SW_UV_BATT", "Battery Under Voltage Alarm", "BATTERY 저전압", "배터리 전압이 하한값 미만입니다.", "전압 하한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000021, "ERR_SW_UV_INPUT1", "Under Voltage Sensing Alarm 1", "입력1 저전압", "센싱 전압이  설정치 보다 낮습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000022, "ERR_SW_UV_INPUT2", "Under Voltage Sensing Alarm 2", "입력2 저전압", "센싱 전압이  설정치 보다 낮습니다", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000030, "ERR_SW_OC_CHGBATT", "Charge Battery Over Current Alarm", "BATTERY 충전 과전류", "배터리 전류가 제한을 초과합니다.", "전류 및 전력 제한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000031, "ERR_SW_OC_DISCBATT", "Discharge Battery Over Current Alarm", "BATTERY 방전 과전류", "배터리 전류가 제한을 초과합니다.", "전류 및 전력 제한을 적절한 값으로 설정하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000032, "ERR_SW_OC_SYNC", "SYNC mode current distribution Alarm", "SYNC 모드 과전류", "SYNC 운전 전류가 균등분배되지 않습니다.", "ACDC로부터 출력공통 단자까지 LINE 및 PBA정상동작을  점검하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000033, "ERR_SW_OC_INPUT1", "Over Current  Sensing Alarm 1", "입력 과전류 1", "입력전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000034, "ERR_SW_OC_OUTPUT1", "Over Current  Sensing Alarm 1", "출력 과전류 1", "출력전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000035, "ERR_SW_OC_INPUT2", "Over Current  Sensing Alarm 2", "입력 과전류 2", "입력전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000036, "ERR_SW_OC_OUTPUT2", "Over Current  Sensing Alarm 2", "출력 과전류 2", "출력전류가 기준치이상으로 초과합니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000037, "", "Operation commend Alarm", "배터리 충방전 지령 에러", "충방전 지령이 잘못되었습니다.", "운전 지령값을 다시 확인하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0x01000038, "", "parallel setting commend Alarm", "병렬지령 설정 에러", "병렬 설정이 잘못되었습니다.", "설정을 다시 확인하십시오");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0xF0000001, "ERR_SW_SafetyOutOfRange", "SafetyOutOfRange", "안전 범위 벗어남", "전압/전류/온도 중 하나 이상이 안전 범위를 벗어났습니다.", "안전 범위를 벗어난 항목에 대한 조치가 필요합니다.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0xF0000002, "ERR_SW_ReturnCodeError", "ReturnCodeError", "패킷 리턴 코드", "방전기로부터 수신 받은 패킷에 오류가 발생하였습니다.", "케이블 연결상태를 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(0xF0000003, "ERR_SW_ChStatusError", "ChStatusError", "방전기 동작 모드", "방전기의 동작 모드가 비정상입니다.", "기기를 끄고 3분 이상 기다렸다가 다시 켜십시오.");
            InsertData(oneRow);
        }

        public static void InsertData(TableDischargerErrorCode oneRowData)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = "";
                query += @"INSERT INTO " + ClassName + " (";
                query += "'" + nameof(oneRowData.Code) + "',";
                query += "'" + nameof(oneRowData.Name) + "',";
                query += "'" + nameof(oneRowData.Title) + "',";
                query += "'" + nameof(oneRowData.Description) + "',";
                query += "'" + nameof(oneRowData.Cause) + "',";
                query += "'" + nameof(oneRowData.Action) + "'";
                query += ") ";
                query += "values (";
                query += "'" + oneRowData.Code + "', ";
                query += "'" + oneRowData.Name + "', ";
                query += "'" + oneRowData.Title + "', ";
                query += "'" + oneRowData.Description + "', ";
                query += "'" + oneRowData.Cause + "', ";
                query += "'" + oneRowData.Action + "'";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<TableDischargerErrorCode> GetData()
        {
            TableDischargerErrorCode instance = new TableDischargerErrorCode();
            List<TableDischargerErrorCode> table = new List<TableDischargerErrorCode>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM " + ClassName;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableDischargerErrorCode oneRow = new TableDischargerErrorCode
                        {
                            Code = uint.Parse(reader[nameof(instance.Code)].ToString()),
                            Name = reader[nameof(instance.Name)].ToString(),
                            Title = reader[nameof(instance.Title)].ToString(),
                            Description = reader[nameof(instance.Description)].ToString(),
                            Cause = reader[nameof(instance.Cause)].ToString(),
                            Action = reader[nameof(instance.Action)].ToString()
                        };
                        table.Add(oneRow);
                    }
                }
            }

            return table;
        }

        public static void DeleteData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"DELETE FROM " + ClassName;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteData(uint code)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"DELETE FROM " + ClassName + " WHERE \"Code\"==" + code + "";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
