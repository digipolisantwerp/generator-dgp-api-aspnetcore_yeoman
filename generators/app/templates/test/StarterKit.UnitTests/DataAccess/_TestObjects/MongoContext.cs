using MongoDB.Driver;
using StarterKit.DataAccess.Context;
using StarterKit.DataAccess.Options;

namespace StarterKit.UnitTests.DataAccess._TestObjects
{
  public class MongoContext : BaseContext
  {
    public MongoContext(DataAccessSettingsMongo options)
     : base (options)
    {
    }

    public IMongoCollection<FooMongo> Foos => Database.GetCollection<FooMongo>("Foos");
  }
}
