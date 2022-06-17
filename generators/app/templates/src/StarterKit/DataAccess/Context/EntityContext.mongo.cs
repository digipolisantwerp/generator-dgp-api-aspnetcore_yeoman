using StarterKit.DataAccess.Options;

namespace StarterKit.DataAccess.Context
{
	public class EntityContextMongo : BaseContext
	{
		public EntityContextMongo(DataAccessSettingsMongo options) : base(options)
		{
		}

		/*
		* add IMongoCollections like so:
		* public IMongoCollection<EntityBase> Entities => _database.GetCollection<EntityBase>("Entities");
		*/
	}
}
