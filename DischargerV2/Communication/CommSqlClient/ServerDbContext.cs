using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SqlClient.Server
{
    // EF6 DbContext for server DB
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer<ServerDbContext>(null);
        }

        public virtual DbSet<TABLE_MST_USER_INFO> MST_USER_INFO { get; set; }
        public virtual DbSet<TABLE_SYS_STS_SDC> SYS_STS_SDC { get; set; }
        public virtual DbSet<TABLE_QLT_HISTORY_ALARM> QLT_HISTORY_ALARM { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: keep original names (annotations already set [Table])
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Only keep mappings that annotations cannot fully express or where we want provider-specific precision.

            // SYS_STS_SDC - varchar(max) columns
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargerName)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargerState)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargerVoltage)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargerCurrent)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargerTemp)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargeCapacity_Ah)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargeCapacity_kWh)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargePhase)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargeMode)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.DischargeTarget)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.LogFileName)
                .HasColumnType("varchar(max)");
            modelBuilder.Entity<TABLE_SYS_STS_SDC>().Property(x => x.ProgressTime)
                .HasColumnType("varchar(max)");

            // QLT_HISTORY_ALARM - set composite key excluding nullable Alarm_Code
            modelBuilder.Entity<TABLE_QLT_HISTORY_ALARM>()
                .HasKey(x => new { x.MC_CD, x.CH_NO, x.Alarm_Time });
        }
    }
}
