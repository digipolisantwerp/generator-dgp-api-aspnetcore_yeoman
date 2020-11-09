using Digipolis.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class TestContext : EntityContextBase<TestContext>
  {
    public TestContext(DbContextOptions<TestContext> options)
      : base(options)
    {
    }
  }
}
