namespace StarterKit.DataAccess.Context
{
  public interface IContext
  {
    void BeginTransaction();
    void Commit();
    void Rollback();
  }
}
