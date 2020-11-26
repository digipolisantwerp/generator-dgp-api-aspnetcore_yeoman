using Microsoft.EntityFrameworkCore;

namespace StarterKit.DataAccess.Context
{
  public class EntityContext : Context.Context<EntityContext>
  {
    public EntityContext(DbContextOptions<EntityContext> options) : base(options)
    {
    }

    // Add your DbSets here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema(DataAccessDefaults.SchemaName); // Remove this if you are not using schema's

      // Your modelBuilder code here
    }
  }
}
