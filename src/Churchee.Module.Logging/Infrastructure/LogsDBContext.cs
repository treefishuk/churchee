using Churchee.Module.Logging.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Logging.Infrastructure
{
    internal class LogsDBContext : DbContext
    {

        public LogsDBContext(DbContextOptions<LogsDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Logs");

                entity.HasIndex(i => i.TimeStamp, "IX_Logs_TimeStamp_DESC").IsDescending(true);
            });
        }
    }
}
