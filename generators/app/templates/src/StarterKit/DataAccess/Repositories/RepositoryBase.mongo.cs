using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StarterKit.DataAccess.Context;
using StarterKit.Entities;

namespace StarterKit.DataAccess.Repositories
{
	public abstract class RepositoryBaseMongo<TEntity, TId> : IRepositoryInjectionMongo<TEntity, TId>
		where TEntity : IEntityBase<TId>
	{
		protected RepositoryBaseMongo(ILogger<DataAccess> logger, ContextBase context)
		{
			Logger = logger;
			Context = context;
			SetEntityCollection();
		}

		protected ILogger Logger { get; private set; }
		public ContextBase Context { get; private set; }

		public IMongoCollection<TEntity> EntityCollection { get; private set; }

		public void SetContext(ContextBase context)
		{
			Context = context;
			SetEntityCollection();
		}

		private void SetEntityCollection()
		{
			if (Context == null)
			{
				EntityCollection = null;
				return;
			}

			var collectionProperty = Context
				.GetType()
				.GetProperties()
				.FirstOrDefault(
					p => p.PropertyType == typeof(IMongoCollection<TEntity>)
				);

			if (collectionProperty == null)
			{
				Logger.LogCritical($"No IMongoCollection {typeof(TEntity)} found on Mongo context {nameof(Context)}");
				throw new Exception($"No IMongoCollection {typeof(TEntity)} found on Mongo context {nameof(Context)}");
			}

			EntityCollection = collectionProperty.GetValue(Context) as IMongoCollection<TEntity>;
		}
	}
}