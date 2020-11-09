using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace StarterKit.DataAccess.Context
{
  public class ContextBase<TContext> : DbContext, IContextBase where TContext : DbContext
  {
    private IDbContextTransaction _transaction;

    public ContextBase(DbContextOptions<TContext> options) : base(options)
    {
    }

    public void BeginTransaction()
    {
      _transaction = Database.BeginTransaction();
    }

    public void Commit()
    {
      try
      {
        SaveChanges();
        _transaction.Commit();
      }
      finally
      {
        _transaction.Dispose();
      }
    }

    public void Rollback()
    {
      _transaction.Rollback();
      _transaction.Dispose();
    }
  }
}
