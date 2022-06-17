using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooRepository : EntityRepositoryBase<InMemoryContext, Foo, FooRepository>, IFooRepository
  {
    public FooRepository(ILogger<FooRepository> logger, InMemoryContext context) : base(logger, context)
    {
    }
  }
}
