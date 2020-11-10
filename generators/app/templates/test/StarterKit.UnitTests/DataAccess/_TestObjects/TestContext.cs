using Microsoft.EntityFrameworkCore;
using StarterKit.DataAccess.Context;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class TestContext : ContextBase<TestContext>
  {
    public TestContext(DbContextOptions<TestContext> options)
      : base(options)
    {
    }
  }
}
