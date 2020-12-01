namespace StarterKit.Shared.Constants
{

  /// <summary>
  /// MongoDB specific environment variables. Use these to override existing settings
  /// </summary>
  public class DataAccessSettingsConfigKeyMongo
  {
    public const string Host = "DB_MONGO_CONNECTION_HOST";
    public const string Port = "DB_MONGO_CONNECTION_PORT";
    public const string DbName = "DB_MONGO_CONNECTION_NAME";
    public const string User = "DB_MONGO_AUTH_USER";
    public const string PassWord = "DB_MONGO_AUTH_PASSWORD";
  }
}
