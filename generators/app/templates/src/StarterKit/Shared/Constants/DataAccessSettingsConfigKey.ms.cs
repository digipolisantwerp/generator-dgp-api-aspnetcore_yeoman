namespace StarterKit.Shared.Constants
{
  /// <summary>
  /// MSSQL specific environment variables. Use these to override existing settings
  /// </summary>
  public class DataAccessSettingsConfigKeyMs
  {
    public const string Host = "DB_MSSQL_CONNECTION_HOST";
    public const string Port = "DB_MSSQL_CONNECTION_PORT";
    public const string DbName = "DB_MSSQL_CONNECTION_NAME";
    public const string User = "DB_MSSQL_AUTH_USER";
    public const string PassWord = "DB_MSSQL_AUTH_PASSWORD";
  }
}
