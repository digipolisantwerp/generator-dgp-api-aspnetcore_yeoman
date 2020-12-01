using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooSqlLiteRepository : EntityRepositoryBase<SqlLiteContext, Foo>, IFooRepository
  {
    public FooSqlLiteRepository(ILogger<StarterKit.DataAccess.DataAccess> logger, SqlLiteContext context) : base(logger, context)
    {
    }
  }
}
