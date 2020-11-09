using Microsoft.Extensions.Logging;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooRepository : EntityRepositoryBase<InMemoryContext, Foo>, IFooRepository
  {
    public FooRepository(ILogger<Digipolis.DataAccess.DataAccess> logger, InMemoryContext context) : base(logger, context)
    {
    }
  }
}
