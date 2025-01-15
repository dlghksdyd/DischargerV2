using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.Database
{
    class DatabaseContext : DbContext
    {
        public static int RequiredDatabaseVersion = 1;
        public DbSet<TblDeviceInfo> dev_info { get; set; }
        public DbSet<TblUserInfo> user_info { get; set; }
        public DbSet<TblBatteryInfo> battery_info { get; set; }
        public static string ConnectionString = @"Data Source=|DataDirectory|\Database.db";
        public static string StaticDataConnectionString = @"Data Source=|DataDirectory|\StaticDatabase.db";
        public DatabaseContext() : base(new SQLiteConnection(ConnectionString), true) { }
        public DatabaseContext(string connectionString) : base(connectionString) { }
        public DatabaseContext(DbConnection conn, bool contextOwnsConnection) : base(conn, contextOwnsConnection) { }

        private static void CreateTables()
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                        TblDeviceInfo(
                                            DevName STRING PRIMARY KEY,
                                            DevCode STRING,
                                            DchrgType STRING,
                                            ChCount STRING,
                                            SpecV STRING,
                                            SpecA STRING,
                                            ConType STRING,
                                            ConInfo STRING,
                                            ComPort STRING,
                                            PortCh STRING,
                                            create_dt DATETIME DEFAULT CURRENT_TIMESTAMP
                                        )";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS 
                                        TblUserInfo(                                            
                                            userid STRING PRIMARY KEY,
                                            password STRING,
                                            username STRING,                                            
                                            create_dt DATETIME DEFAULT CURRENT_TIMESTAMP
                                        )";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS 
                                        TblBatteryInfo(                                            
                                            BatteryType STRING PRIMARY KEY,
                                            Voltage STRING,
                                            Capacity STRING,       
                                            CathodeType STRING,       
                                            Configuration STRING,       
                                            create_dt DATETIME DEFAULT CURRENT_TIMESTAMP
                                        )";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }
        }

        public static void Initialize()
        {
            CreateTables();
        }

        #region TblUserInfo Query
        public static void Insert(TblUserInfo userInfo)
        {
            using (var conn = new DatabaseContext())
            {
                try
                {
                    conn.user_info.Add(userInfo);
                    conn.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static TblUserInfo SelectOne(string id, string passwd)
        {
            TblUserInfo result = null;
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"SELECT userid, password, username, create_dt 
                                        FROM TblUserInfo WHERE userid='" + id +
                                        "' AND password='" + passwd + "'";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var userInfo = new TblUserInfo();
                            userInfo.UserId = reader["userid"].ToString();
                            userInfo.Password = reader["password"].ToString();
                            userInfo.UserName = reader["username"].ToString();
                            userInfo.CreateDt = Convert.ToDateTime(reader["create_dt"]);
                            result = userInfo;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return result;
        }

        public static void UpdateUserInfo(TblUserInfo userInfo)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"UPDATE tblUserInfo set password ='" + userInfo.Password + "', username='" + userInfo.UserName +
                        "' WHERE userid='" + userInfo.UserId + "'";
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return;
        }

        public static List<TblUserInfo> SelectAllUserinfo()
        {
            var result = new List<TblUserInfo>();

            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"SELECT userid, password, username, create_dt 
                                        FROM TblUserInfo";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var userInfo = new TblUserInfo();
                            userInfo.UserId = reader["userid"].ToString();
                            userInfo.Password = reader["password"].ToString();
                            userInfo.UserName = reader["username"].ToString();
                            userInfo.CreateDt = Convert.ToDateTime(reader["create_dt"]);
                            result.Add(userInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return result;
            //userInfos = result.Count > 1 ? result : null;
        }

        public static bool InsertDbInfo(string sqlStr)
        {
            if (sqlStr != "")
            {
                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    try
                    {
                        var cmd = new SQLiteCommand(con);
                        cmd.CommandText = sqlStr;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        con.Close();
                        return false;
                    }
                    con.Close();
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool DeleteUserInfo(string id)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"DELETE FROM TblUserInfo
                                        WHERE userid='" + id + "'";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    con.Close();
                    return false;
                }
                con.Close();
            }
            return true;
        }
        #endregion

        #region TblDeviceInfo Query
        public static void Insert(TblDeviceInfo devInfo)
        {
            using (var conn = new DatabaseContext())
            {
                try
                {
                    conn.dev_info.Add(devInfo);
                    conn.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static List<TblDeviceInfo> SelectAllDeviceinfo()
        {
            var result = new List<TblDeviceInfo>();

            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"SELECT DevName, DevCode, DchrgType, ChCount, SpecV, SpecA, ConType, ConInfo, Comport, PortCh, create_dt 
                                        FROM TblDeviceInfo";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var devInfo = new TblDeviceInfo();
                            devInfo.DevName = reader["DevName"].ToString();
                            devInfo.DevCode = reader["DevCode"].ToString();
                            devInfo.DchrgType = reader["DchrgType"].ToString();
                            devInfo.ChCount = reader["ChCount"].ToString();
                            devInfo.SpecV = reader["SpecV"].ToString();
                            devInfo.SpecA = reader["SpecA"].ToString();
                            devInfo.ConType = reader["ConType"].ToString();
                            devInfo.ConInfo = reader["ConInfo"].ToString();
                            devInfo.ComPort = reader["ComPort"].ToString();
                            devInfo.PortCh = reader["PortCh"].ToString();
                            devInfo.CreateDt = Convert.ToDateTime(reader["create_dt"]);
                            result.Add(devInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return result;
            //userInfos = result.Count > 1 ? result : null;
        }

        public static void UpdateDevInfo(TblDeviceInfo devInfo)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"UPDATE tblDeviceInfo set DevCode ='" + devInfo.DevCode + "', DchrgType='" + devInfo.DchrgType + "', ChCount='" + devInfo.ChCount +
                        "', SpecV='" + devInfo.SpecV + "', SpecA='" + devInfo.SpecA + "', ConInfo='" + devInfo.ConInfo + "', ConType='" + devInfo.ConType +
                       "', ComPort='" + devInfo.ComPort + "', PortCh='" + devInfo.PortCh + "' WHERE DevName='" + devInfo.DevName + "'";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return;
        }

        public static bool DeleteDevInfo(string devName)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"DELETE FROM TblDeviceInfo
                                        WHERE DevName='" + devName + "'";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    con.Close();
                    return false;
                }
                con.Close();
            }
            return true;
        }
        #endregion

        #region TblBatteryInfo Query
        public static void Insert(TblBatteryInfo batteryInfo)
        {
            using (var conn = new DatabaseContext())
            {
                try
                {
                    conn.battery_info.Add(batteryInfo);
                    conn.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void UpdateBatteryInfo(TblBatteryInfo batteryInfo)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    /*cmd.CommandText = @"UPDATE password ='" + userInfo.Password + ", username='" + userInfo.UserName +
                        "FROM TblUserInfo WHERE userid='" + userInfo.UserId + "'";*/
                    cmd.CommandText = @"UPDATE tblBatteryInfo set Voltage ='" + batteryInfo.Voltage + "', Capacity='" + batteryInfo.Capacity +
                        "', CathodeType='" + batteryInfo.CathodeType + "', Configuration='" + batteryInfo.Configuration + "' WHERE BatteryType='" + batteryInfo.BatteryType + "'";
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return;
        }

        public static List<TblBatteryInfo> SelectAllBatteryinfo()
        {
            var result = new List<TblBatteryInfo>();

            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"SELECT BatteryType, Voltage, Capacity, CathodeType, Configuration, create_dt 
                                        FROM TblBatteryInfo";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var batteryInfo = new TblBatteryInfo();
                            batteryInfo.BatteryType = reader["BatteryType"].ToString();
                            batteryInfo.Voltage = reader["Voltage"].ToString();
                            batteryInfo.Capacity = reader["Capacity"].ToString();
                            batteryInfo.CathodeType = reader["CathodeType"].ToString();
                            batteryInfo.Configuration = reader["Configuration"].ToString();
                            batteryInfo.CreateDt = Convert.ToDateTime(reader["create_dt"]);
                            result.Add(batteryInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return result;
            //userInfos = result.Count > 1 ? result : null;
        }

        public static bool DeleteBatteryInfo(string batteryType)
        {
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"DELETE FROM TblBatteryInfo
                                        WHERE batteryType='" + batteryType + "'";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    con.Close();
                    return false;
                }
                con.Close();
            }
            return true;
        }
        #endregion

        #region TblDischargerTypeInfo Query
        public static List<TblDischargerTypeInfo> SelectAllDischargerTypeInfo()
        {
            var result = new List<TblDischargerTypeInfo>();

            using (var con = new SQLiteConnection(StaticDataConnectionString))
            {
                con.Open();
                try
                {
                    var cmd = new SQLiteCommand(con);
                    cmd.CommandText = @"SELECT DevCode, DchrgType, ChCount, SpecV, SpecA, SafetyHV, SafetyLV, SafetyHA, SafetyLA, SafetyHT, SafetyLT 
                                        FROM TblDischargerTypeInfo";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dischargerTypeInfo = new TblDischargerTypeInfo();
                            dischargerTypeInfo.DevCode = reader["DevCode"].ToString();
                            dischargerTypeInfo.DchrgType = reader["DchrgType"].ToString();
                            dischargerTypeInfo.ChCount = reader["ChCount"].ToString();
                            dischargerTypeInfo.SpecV = reader["SpecV"].ToString();
                            dischargerTypeInfo.SpecA = reader["SpecA"].ToString();
                            dischargerTypeInfo.SafetyHV = reader["SafetyHV"].ToString();
                            dischargerTypeInfo.SafetyLV = reader["SafetyLV"].ToString();
                            dischargerTypeInfo.SafetyHA = reader["SafetyHA"].ToString();
                            dischargerTypeInfo.SafetyLA = reader["SafetyLA"].ToString();
                            dischargerTypeInfo.SafetyHT = reader["SafetyHT"].ToString();
                            dischargerTypeInfo.SafetyLT = reader["SafetyLT"].ToString();
                            result.Add(dischargerTypeInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                con.Close();
            }

            return result;
            //userInfos = result.Count > 1 ? result : null;
        }
        #endregion
    }
}
