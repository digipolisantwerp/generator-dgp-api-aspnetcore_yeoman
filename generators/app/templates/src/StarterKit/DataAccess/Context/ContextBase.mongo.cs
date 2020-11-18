using MongoDB.Driver;
using StarterKit.DataAccess.Options;

namespace StarterKit.DataAccess.Context
{
  public class ContextMongo : IContext
  {
    private IClientSessionHandle _transactionSession;
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public ContextMongo(DataAccessSettingsMongo options)
    {
      _client = new MongoClient(options.GetConnectionString());
      _database = _client.GetDatabase(options.DbName);
    }

    /// <summary>
    /// add IMongoCollections here like so:
    ///
    /// public IMongoCollection<EntityBase> Entities => _database.GetCollection<EntityBase>("Entities");
    /// </summary>


    public void BeginTransaction()
    {
      _transactionSession = _client.StartSession();
      _transactionSession.StartTransaction();
    }

    public void Commit()
    {
      try
      {
        _transactionSession.CommitTransaction();
      }
      finally
      {
        _transactionSession.Dispose();
      }
    }

    public void Rollback()
    {
      _transactionSession.AbortTransactionAsync();
      _transactionSession.Dispose();
    }
  }
}
