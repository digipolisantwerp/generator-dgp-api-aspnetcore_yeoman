using Digipolis.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using StarterKit.Entities;

namespace StarterKit.DataAccess
{
   public class EntityContext : EntityContextBase<EntityContext>
    {
        private readonly ILoggerFactory _loggerFactory;

        public EntityContext(DbContextOptions<EntityContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        // Add your DbSets here
        public DbSet<MyEntity> MyEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DataAccessDefaults.SchemaName);       // Remove this if you are not using schema's

            // Your modelBuilder code here
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            if (Log.IsEnabled(LogEventLevel.Debug))
            {
              optionsBuilder.EnableSensitiveDataLogging();
            }
        }
    }
}
