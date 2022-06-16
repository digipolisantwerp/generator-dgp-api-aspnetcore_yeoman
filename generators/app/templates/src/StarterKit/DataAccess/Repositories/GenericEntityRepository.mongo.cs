using Microsoft.Extensions.Logging;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
	public class GenericEntityRepositoryMongo<TEntity> : EntityRepositoryBaseMongo<TEntity, string>
		where TEntity : class, IEntityBase<string>, new()
	{
		public GenericEntityRepositoryMongo(ILogger<DataAccess> logger) : base(logger, null)
		{
		}
	}

	public class GenericEntityRepositoryMongo<TEntity, TId> : EntityRepositoryBaseMongo<TEntity, TId>
		where TEntity : class, IEntityBase<TId>, new()
	{
		public GenericEntityRepositoryMongo(ILogger<DataAccess> logger) : base(logger, null)
		{
		}
	}
}