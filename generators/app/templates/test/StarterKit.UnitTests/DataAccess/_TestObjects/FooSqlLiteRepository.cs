using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooSqlLiteRepository : EntityRepositoryBase<SqlLiteContext, Foo, FooSqlLiteRepository>, IFooRepository
  {
    public FooSqlLiteRepository(ILogger<FooSqlLiteRepository> logger, SqlLiteContext context) : base(logger, context)
    {
    }
  }
}
