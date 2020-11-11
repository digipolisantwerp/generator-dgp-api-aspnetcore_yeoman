using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Context;

namespace StarterKit.DataAccess.Repositories
{
  public abstract class RepositoryBase<TContext> : IRepositoryInjection where TContext : DbContext
  {
    protected RepositoryBase(ILogger<DataAccess> logger, IServiceScopeFactory serviceScopeFactory)
    {
      this.Logger = logger;
      _serviceScopeFactory = serviceScopeFactory;
    }

    protected ILogger Logger { get; private set; }
    public TContext Context { get; private set; }

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public virtual TContext SetNewContext(bool trackChanges = true)
    {
      var context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<TContext>();

      if (!trackChanges)
      {
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      }

      SetContext(context);

      return context;
    }

    public IRepositoryInjection SetContext(DbContext context)
    {
      this.Context = (TContext) context;
      return this;
    }
  }
}
