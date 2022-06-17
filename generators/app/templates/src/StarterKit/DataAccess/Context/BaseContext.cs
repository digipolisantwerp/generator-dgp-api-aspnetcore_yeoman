using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace StarterKit.DataAccess.Context
{
	public abstract class BaseContext<TContext> : DbContext, IContext where TContext : DbContext
	{
		private IDbContextTransaction _transaction;

		public BaseContext(DbContextOptions<TContext> options) : base(options)
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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.LowerCaseTablesAndFields();

			base.OnModelCreating(modelBuilder);
		}
	}
}
