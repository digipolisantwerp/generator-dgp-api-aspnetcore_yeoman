using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterKit.DataAccess.Context;

namespace StarterKit.DataAccess.Repositories
{
  public abstract class RepositoryBase<TContext> : IRepositoryInjection where TContext : DbContext
  {
    protected RepositoryBase(ILogger<DataAccess> logger, TContext context)
    {
      this.Logger = logger;
      Context = context;
    }

    protected ILogger Logger { get; private set; }
    public TContext Context { get; private set; }

    public IRepositoryInjection SetContext(DbContext context)
    {
      this.Context = (TContext) context;
      return this;
    }
  }
}
