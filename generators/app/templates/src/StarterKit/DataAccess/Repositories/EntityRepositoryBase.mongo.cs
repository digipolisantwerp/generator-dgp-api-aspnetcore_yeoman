using Digipolis.Paging.Predicates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StarterKit.DataAccess.Context;
using StarterKit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StarterKit.DataAccess.Repositories
{
	public abstract class EntityRepositoryBaseMongo<TEntity> : RepositoryBaseMongo<TEntity, string>
		where TEntity : class, IEntityBase<string>, new()
	{
		protected EntityRepositoryBaseMongo(ILogger<DataAccess> logger, BaseContext context) : base(logger, context)
		{
		}
	}

	public abstract class EntityRepositoryBaseMongo<TEntity, TId> : RepositoryBaseMongo<TEntity, TId>,
		IRepositoryMongo<TEntity, TId>
		where TEntity : class, IEntityBase<TId>, new()
	{
		protected EntityRepositoryBaseMongo(ILogger<DataAccess> logger, BaseContext context) : base(logger, context)
		{
		}

		public virtual IEnumerable<TEntity> GetAll(string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			return result.ToList();
		}

		public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			return await result.ToListAsync();
		}

		public virtual IEnumerable<TEntity> GetPage(int startRow, int pageLength,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			return result.Skip(startRow).Take(pageLength).ToList();
		}

		public virtual async Task<IEnumerable<TEntity>> GetPageAsync(int startRow, int pageLength,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			return await result.Skip(startRow).Take(pageLength).ToListAsync();
		}

		public virtual TEntity Get(TId id,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (includes != null)
			{
				query = includes(query);
			}

			return query.SingleOrDefault(x => Equals(x.Id, id));
		}

		public virtual Task<TEntity> GetAsync(TId id,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (includes != null)
			{
				query = includes(query);
			}

			return query.SingleOrDefaultAsync(x => Equals(x.Id, id));
		}

		public virtual IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			return result.ToList();
		}

		public virtual async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			return await result.ToListAsync();
		}

		public virtual void Load(Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			result.Load();
		}

		public virtual async Task LoadAsync(Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			await result.LoadAsync();
		}

		public virtual IEnumerable<TEntity> QueryPage(int startRow, int pageLength,
			Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			return result.Skip(startRow).Take(pageLength).ToList();
		}

		public virtual async Task<IEnumerable<TEntity>> QueryPageAsync(int startRow, int pageLength,
			Expression<Func<TEntity, bool>> filter,
			string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(filter, includes, sortString);
			return await result.Skip(startRow).Take(pageLength).ToListAsync();
		}

		public virtual void Add(TEntity entity)
		{
			if (entity == null) throw new InvalidOperationException("Unable to add a null entity to the repository.");
			EntityCollection.InsertOne(entity);
		}

		public virtual void AddBatch(IEnumerable<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			EntityCollection.InsertMany(entities);
		}

		public virtual bool Update(TEntity entity)
		{
			var actionResult = EntityCollection
				.ReplaceOne(n => n.Id.Equals(entity.Id),
					entity,
					new ReplaceOptions
					{
						IsUpsert = true
					});
			return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
		}

		public virtual bool UpdateBatch(IEnumerable<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			return entities.Select(Update).All(a => a);
		}

		public virtual bool Remove(TEntity entity)
		{
			return Remove(entity.Id);
		}

		public virtual bool RemoveBatch(IEnumerable<TEntity> entities)
		{
			return RemoveBatch(entities.Select(i => i.Id));
		}

		public virtual bool Remove(TId id)
		{
			var actionResult = EntityCollection.DeleteOne(
				e => e.Id.Equals(id));

			return actionResult.IsAcknowledged
			       && actionResult.DeletedCount > 0;
		}

		public virtual bool RemoveBatch(IEnumerable<TId> ids)
		{
			if (ids == null) throw new ArgumentNullException(nameof(ids));
			var actionResult = EntityCollection.DeleteMany(
				e => ids.Contains(e.Id));

			return actionResult.IsAcknowledged
			       && actionResult.DeletedCount > 0;
		}

		public virtual bool Any(Expression<Func<TEntity, bool>> filter = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return query.Any();
		}

		public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return query.AnyAsync();
		}

		public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return query.Count();
		}

		public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return query.CountAsync();
		}

		public virtual void Load(string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			result.Load();
		}

		public virtual async Task LoadAsync(string sortString = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
		{
			var result = QueryDb(null, includes, sortString);
			await result.LoadAsync();
		}

		private Expression<Func<TEntity, bool>> CombineExpressions(
			Expression<Func<TEntity, bool>> defExpression, Expression<Func<TEntity, bool>> includes)
		{
			if (includes == null)
			{
				return defExpression;
			}

			var body = Expression.AndAlso(includes.Body, defExpression.Body);
			return Expression.Lambda<Func<TEntity, bool>>(body, defExpression.Parameters[0]);
		}

		protected IQueryable<TEntity> QueryDb(Expression<Func<TEntity, bool>> filter,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> includes,
			string sortString = null)
		{
			IQueryable<TEntity> query = EntityCollection.AsQueryable();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includes != null)
			{
				query = includes(query);
			}

			query = query.OrderBy(sortString);

			return query;
		}
	}
}
