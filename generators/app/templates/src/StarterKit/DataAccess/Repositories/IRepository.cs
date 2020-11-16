using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StarterKit.DataAccess.Repositories
{
  public interface IRepository<TEntity, in TId>
  {
    IEnumerable<TEntity> GetAll(string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    Task<IEnumerable<TEntity>> GetAllAsync(string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    IEnumerable<TEntity> GetPage(int startRow, int pageLength,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    Task<IEnumerable<TEntity>> GetPageAsync(int startRow, int pageLength,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    TEntity Get(TId id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);
    Task<TEntity> GetAsync(TId id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    IEnumerable<TEntity> QueryPage(int startRow, int pageLength, Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    Task<IEnumerable<TEntity>> QueryPageAsync(int startRow, int pageLength, Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    void Load(Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    Task LoadAsync(Expression<Func<TEntity, bool>> filter,
      string sortString = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null);

    void Add(TEntity entity);

    TEntity Update(TEntity entity);

    void Remove(TEntity entity);
    void Remove(TId id);

    bool Any(Expression<Func<TEntity, bool>> filter = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null);

    int Count(Expression<Func<TEntity, bool>> filter = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);

    void SetUnchanged(TEntity entity);
    void AddBatch(IEnumerable<TEntity> entities);
    IEnumerable<TEntity> UpdateBatch(IEnumerable<TEntity> entities);
    void RemoveBatch(IEnumerable<TEntity> entities);
    void RemoveBatch(IEnumerable<TId> ids);
  }
}
