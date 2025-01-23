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

        public static void InsertData(TableUserInfo oneRowData)
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

        public static void UpdateData(TableUserInfo oneRowData)
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
        }

        public static void DeleteData(string userId)
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
                query += "'Id' INTEGER AUTO INCREMENT PRIMARY KEY, ";
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
                query += "'SafetyTempMin' REAL";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertData(TableDischargerModel oneRowData)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = "";
                query += @"INSERT INTO " + ClassName + " (";
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

        public static void DeleteData(int id)
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
                query += "'TempModuleComPort' TEXT, ";
                query += "'TempModuleChannel' INTEGER, ";
                query += "'TempChannel' INTEGER";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateData(TableDischargerInfo oneRowData)
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
                query += "\"TempModuleComPort\"='" + oneRowData.TempModuleComPort + "',";
                query += "\"TempModuleChannel\"='" + oneRowData.TempModuleChannel + "', ";
                query += "\"TempChannel\"='" + oneRowData.TempChannel + "'";
                query += "WHERE \"Name\"='" + oneRowData.DischargerName + "'";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertData(TableDischargerInfo oneRowData)
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
                query += "'" + oneRowData.TempModuleComPort + "', ";
                query += "'" + oneRowData.TempModuleChannel + "', ";
                query += "'" + oneRowData.TempChannel + "'";
                query += ")";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
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
                        oneRow.TempModuleComPort = reader["TempModuleComPort"].ToString();
                        oneRow.TempModuleChannel = int.Parse(reader["TempModuleChannel"].ToString());
                        oneRow.TempChannel = int.Parse(reader["TempChannel"].ToString());
                        table.Add(oneRow);
                    }
                }
            }

            return table;
        }

        public static void DeleteData(string name)
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

            oneRow = new TableDischargerErrorCode(
                0x01001000, "ERR_HW_INPUT_REV", 
                "Cable reverse connection Alarm", "Cable 역결선", 
                "전류케이블 극성이 반대입니다.", "전류케이블 극성을 확인하십시오.");
            InsertData(oneRow);

            oneRow = new TableDischargerErrorCode(
                0x01001100, "ERR_HW_INPUT_CABLE",
                "Cable connection Alarm", "Cable 결선 오류",
                "결선을 확인하십시오.", "전류CABLE 또는, 배터리 센싱선 결선을 확인하십시오.");
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
