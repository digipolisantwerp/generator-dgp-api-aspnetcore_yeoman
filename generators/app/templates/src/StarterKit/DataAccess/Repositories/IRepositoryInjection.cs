using Microsoft.EntityFrameworkCore;

namespace StarterKit.DataAccess.Repositories
{
  public interface IRepositoryInjection
  {
    IRepositoryInjection SetContext(DbContext context);
  }
}
