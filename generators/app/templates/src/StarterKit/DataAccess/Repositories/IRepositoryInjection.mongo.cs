using MongoDB.Driver;
using StarterKit.DataAccess.Context;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
  public interface IRepositoryInjectionMongo<TEntity, in TId> where TEntity : IEntityBase<TId>
  {
    void SetContext(ContextMongo context);
  }
}
