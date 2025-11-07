using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public static TABLE_MST_USER_INFO FindUserInfo(string userId, string password)
        {
            try
            {
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    // Trim inputs for comparison consistency
                    userId = userId?.Trim();
                    password = password?.Trim();

                    var user = ctx.MST_USER_INFO
                        .FirstOrDefault(x => x.USER_ID == userId && x.PASSWD == password);

                    if (user == null) return null;

                    // Normalize string values as original logic did (Trim)
                    return new TABLE_MST_USER_INFO
                    {
                        SERIAL = user.SERIAL,
                        USER_ID = user.USER_ID?.Trim(),
                        USER_NM = user.USER_NM?.Trim(),
                        PASSWD = user.PASSWD?.Trim(),
                        ADMIN_GROUP = user.ADMIN_GROUP?.Trim(),
                        USE_YN = user.USE_YN?.Trim(),
                    };
                }
            }
            catch
            {
                return null;
            }
        }

        public static List<TABLE_MST_USER_INFO> GetData()
        {
            try
            {
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    // Materialize then trim to mimic previous behavior
                    return ctx.MST_USER_INFO
                        .AsEnumerable()
                        .Select(u => new TABLE_MST_USER_INFO
                        {
                            SERIAL = u.SERIAL,
                            USER_ID = u.USER_ID?.Trim(),
                            USER_NM = u.USER_NM?.Trim(),
                            PASSWD = u.PASSWD?.Trim(),
                            ADMIN_GROUP = u.ADMIN_GROUP?.Trim(),
                            USE_YN = u.USE_YN?.Trim(),
                        })
                        .ToList();
                }
            }
            catch
            {
                return new List<TABLE_MST_USER_INFO>();
            }
        }
    }

    public static class SqlClientStatus
    {
        public static bool InsertData_Init(TABLE_SYS_STS_SDC data)
        {
            try
            {
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var entity = ctx.SYS_STS_SDC
                        .FirstOrDefault(x => x.MC_CD == data.MC_CD && x.MC_CH == data.MC_CH);

                    if (entity != null)
                    {
                        entity.DischargerName = data.DischargerName;
                        entity.MC_DTM = DateTime.Now;
                    }
                    else
                    {
                        entity = new TABLE_SYS_STS_SDC
                        {
                            MC_CD = data.MC_CD,
                            MC_CH = data.MC_CH,
                            DischargerName = data.DischargerName,
                            MC_DTM = DateTime.Now,
                        };
                        ctx.SYS_STS_SDC.Add(entity);
                    }

                    ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var entity = ctx.SYS_STS_SDC
                        .FirstOrDefault(x => x.MC_CD == data.MC_CD && x.MC_CH == data.MC_CH);

                    if (entity != null)
                    {
                        entity.USER_ID = data.USER_ID;
                        entity.DischargerVoltage = data.DischargerVoltage;
                        entity.DischargerCurrent = data.DischargerCurrent;
                        entity.DischargerTemp = data.DischargerTemp;
                        entity.MC_DTM = DateTime.Now;

                        ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var entity = ctx.SYS_STS_SDC
                        .FirstOrDefault(x => x.MC_CD == data.MC_CD && x.MC_CH == data.MC_CH);

                    if (entity != null)
                    {
                        entity.USER_ID = data.USER_ID;
                        entity.DischargerState = data.DischargerState;
                        entity.ProgressTime = data.ProgressTime;
                        entity.MC_DTM = DateTime.Now;

                        ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var entity = ctx.SYS_STS_SDC
                        .FirstOrDefault(x => x.MC_CD == data.MC_CD && x.MC_CH == data.MC_CH);

                    if (entity != null)
                    {
                        entity.USER_ID = data.USER_ID;
                        entity.DischargeMode = data.DischargeMode;
                        entity.DischargeTarget = data.DischargeTarget;
                        entity.LogFileName = data.LogFileName;
                        entity.MC_DTM = DateTime.Now;

                        ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var entity = ctx.SYS_STS_SDC
                        .FirstOrDefault(x => x.MC_CD == data.MC_CD && x.MC_CH == data.MC_CH);

                    if (entity != null)
                    {
                        entity.USER_ID = data.USER_ID;
                        entity.DischargeCapacity_Ah = data.DischargeCapacity_Ah;
                        entity.DischargeCapacity_kWh = data.DischargeCapacity_kWh;
                        entity.DischargePhase = data.DischargePhase;
                        entity.MC_DTM = DateTime.Now;

                        ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    // IFT_HISTORY_ALARM SP logic with EF:
                    // Insert when NOT EXISTS for the tuple (MTYPE, MC_CD, Alarm_Time, Alarm_Desc)
                    string mtype = string.IsNullOrWhiteSpace(data.MTYPE) ? "SDC" : data.MTYPE;

                    bool exists = ctx.QLT_HISTORY_ALARM.Any(x =>
                        x.MTYPE == mtype &&
                        x.MC_CD == data.MC_CD &&
                        x.Alarm_Time == data.Alarm_Time &&
                        x.Alarm_Desc == data.Alarm_Desc);

                    if (!exists)
                    {
                        var row = new TABLE_QLT_HISTORY_ALARM
                        {
                            MTYPE = mtype,
                            MC_CD = data.MC_CD,
                            CH_NO = data.CH_NO,
                            Alarm_Treat = data.Alarm_Treat,
                            Alarm_Time = data.Alarm_Time,
                            Alarm_Code = data.Alarm_Code,
                            Alarm_Desc = data.Alarm_Desc,
                            Alarm_NewInserted = data.Alarm_NewInserted,
                        };
                        ctx.QLT_HISTORY_ALARM.Add(row);
                        ctx.SaveChanges();
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
                using (var ctx = new ServerDbContext(SqlClient.ConnectionString))
                {
                    var rows = ctx.SYS_STS_SDC.Where(x => x.MC_CD == machineCode).ToList();
                    if (rows.Count > 0)
                    {
                        ctx.SYS_STS_SDC.RemoveRange(rows);
                        ctx.SaveChanges();
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
