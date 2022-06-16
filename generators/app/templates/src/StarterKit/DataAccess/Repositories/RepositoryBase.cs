using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StarterKit.DataAccess.Repositories
{
	public abstract class RepositoryBase<TContext> : IRepositoryInjection where TContext : DbContext
	{
		protected RepositoryBase(ILogger<DataAccess> logger, TContext context)
		{
			Logger = logger;
			Context = context;
		}

		protected ILogger Logger { get; private set; }
		public TContext Context { get; private set; }

		public IRepositoryInjection SetContext(DbContext context)
		{
			Context = (TContext)context;
			return this;
		}
	}
}