using Microsoft.EntityFrameworkCore;
using SchwiBot.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchwiBot.Database
{
    public class SchwiDatabaseContext : DbContext
    {
        public DbSet<DbUserStorage> UserStorages { get; set; }
        public DbSet<DbGuildStorage> GuildStorages { get; set; }

        // TODO: move this embedded connection string out to some config option
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=schwi.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DbUserStorage>().Property(b => b.Storage).HasJsonConversion();
            modelBuilder.Entity<DbGuildStorage>().Property(b => b.Storage).HasJsonConversion();
            modelBuilder.Entity<DbGuildStorage>().Property(b => b.UserStorage).HasJsonConversion();
        }
    }

    public class DbUserStorage
    {
        // EF Core sets this as an auto-increment by default but that doesn't make sense here so we force it not to.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public ulong Snowflake { get; private set; }

        [Required]
        public Dictionary<string, string> Storage { get; private set; }

        internal DbUserStorage(ulong snowflake)
        {
            Snowflake = snowflake;
            Storage = new Dictionary<string, string>();
        }
    }

    public class DbGuildStorage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public ulong Snowflake { get; private set; }

        [Required]
        public Dictionary<string, string> Storage { get; private set; }

        [Required]
        public Dictionary<ulong, Dictionary<string, string>> UserStorage { get; private set; }

        internal DbGuildStorage(ulong snowflake)
        {
            Snowflake = snowflake;
            Storage = new Dictionary<string, string>();
            UserStorage = new Dictionary<ulong, Dictionary<string, string>>();
        }
    }
}
