using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
	public class GenericEntityRepository<TEntity> : EntityRepositoryBase<DbContext, TEntity, GenericEntityRepository<TEntity>, int>
		where TEntity : class, IEntityBase<int>, new()
	{
		public GenericEntityRepository(ILogger<GenericEntityRepository<TEntity>> logger) : base(logger, null)
		{
		}
	}

	public class GenericEntityRepository<TEntity, TId> : EntityRepositoryBase<DbContext, TEntity, GenericEntityRepository<TEntity, TId>, TId>
		where TEntity : class, IEntityBase<TId>, new()
	{
		public GenericEntityRepository(ILogger<GenericEntityRepository<TEntity, TId>> logger) : base(logger, null)
		{
		}
	}
}
