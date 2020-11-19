using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Repositories;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class FooMongoRepository : EntityRepositoryBaseMongo<FooMongo>, IFooRepository
  {
    public FooMongoRepository(ILogger<StarterKit.DataAccess.DataAccess> logger, MongoContext context) : base(logger, context)
    {
    }
  }
}
