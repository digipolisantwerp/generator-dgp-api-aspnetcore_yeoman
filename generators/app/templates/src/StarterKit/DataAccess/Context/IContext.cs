namespace StarterKit.DataAccess.Context
{
  interface IContext
  {
    void BeginTransaction();
    void Commit();
    void Rollback();
  }
}
