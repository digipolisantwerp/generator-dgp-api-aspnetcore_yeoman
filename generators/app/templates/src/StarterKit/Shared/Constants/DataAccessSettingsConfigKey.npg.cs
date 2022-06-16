namespace StarterKit.Shared.Constants
{
	/// <summary>
	/// PostgreSQL specific environment variables. Use these to override existing settings
	/// </summary>
	public class DataAccessSettingsConfigKeyNpg
	{
		public const string Host = "DB_POSTGRESQL_CONNECTION_HOST";
		public const string Port = "DB_POSTGRESQL_CONNECTION_PORT";
		public const string DbName = "DB_POSTGRESQL_CONNECTION_NAME";
		public const string User = "DB_POSTGRESQL_AUTH_USER";
		public const string PassWord = "DB_POSTGRESQL_AUTH_PASSWORD";
		public const string DryRun = "DB_POSTGRESQL_MIGRATIONS_DRYRUN";
		public const string MarkAllAsExecuted = "DB_POSTGRESQL_MIGRATIONS_MARKALLASEXECUTED";
		public const string RunScripts = "DB_POSTGRESQL_MIGRATIONS_RUNSCRIPTS";
	}
}
