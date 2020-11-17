using Microsoft.EntityFrameworkCore;
using StarterKit.DataAccess.Context;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class SqlLiteContext : Context<SqlLiteContext>
  {
    public SqlLiteContext()
      : base(new DbContextOptions<SqlLiteContext>())
    {
    }

    public SqlLiteContext(DbContextOptions<SqlLiteContext> options)
      : base(options)
    {
    }

    public DbSet<Foo> Foos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
      => options.UseSqlite("Data Source=TestDatabase.db");

  }
}
