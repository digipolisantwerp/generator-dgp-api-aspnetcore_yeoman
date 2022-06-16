using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
	public class GenericEntityRepository<TEntity> : EntityRepositoryBase<DbContext, TEntity, int>
		where TEntity : class, IEntityBase<int>, new()
	{
		public GenericEntityRepository(ILogger<DataAccess> logger) : base(logger, null)
		{
		}
	}

	public class GenericEntityRepository<TEntity, TId> : EntityRepositoryBase<DbContext, TEntity, TId>
		where TEntity : class, IEntityBase<TId>, new()
	{
		public GenericEntityRepository(ILogger<DataAccess> logger) : base(logger, null)
		{
		}
	}
}