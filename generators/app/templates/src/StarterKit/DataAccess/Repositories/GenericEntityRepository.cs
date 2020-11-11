using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Context;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
  public class GenericEntityRepository<TEntity> : EntityRepositoryBase<DbContext, TEntity, int>
    where TEntity : class, IEntityBase<int>, new()
  {
    public GenericEntityRepository(ILogger<DataAccess> logger, IServiceScopeFactory serviceScopeFactory = null) : base(logger, serviceScopeFactory)
    {
    }
  }

  public class GenericEntityRepository<TEntity, TId> : EntityRepositoryBase<DbContext, TEntity, TId>
    where TEntity : class, IEntityBase<TId>, new()
  {
    public GenericEntityRepository(ILogger<DataAccess> logger, IServiceScopeFactory serviceScopeFactory = null) : base(logger, serviceScopeFactory)
    {
    }
  }
}
