using Digipolis.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class InMemoryContext : EntityContextBase<InMemoryContext>
  {
    public InMemoryContext(DbContextOptions<InMemoryContext> options)
      : base(options)
    {
    }

    public DbSet<Foo> Foos { get; set; }

    public static InMemoryContext Create()
    {
      // Create a fresh service provider, and therefore a fresh
      // InMemory database instance.
      var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

      // Create a new options instance telling the context to use an
      // InMemory database and the new service provider.
      var builder = new DbContextOptionsBuilder<InMemoryContext>();
      builder.UseInMemoryDatabase("TestDatabase")
        .UseInternalServiceProvider(serviceProvider);

      return new InMemoryContext(builder.Options);
    }
  }
}
