namespace StarterKit.DataAccess.Context
{
  interface IContextBase
  {
    void BeginTransaction();
    void Commit();
    void Rollback();
  }
}
