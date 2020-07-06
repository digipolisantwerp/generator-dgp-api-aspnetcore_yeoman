using System;
using Digipolis.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StarterKit.DataAccess.Options
{
  public class DataAccessSettingsMs
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string DbName { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section)
    {
      services.Configure<DataAccessSettingsMs>(settings =>
      {
        settings.LoadFromConfigSection(section);
        settings.OverrideFromEnvironmentVariables();
      });
    }

    public string GetConnectionString()
    {
      ushort port = 0;
      try
      {
        port = ushort.Parse(Port);
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException("Database port must be a number from 0 to 65536.", ex.InnerException ?? ex);
      }

      var connectionString = new ConnectionString(Host, port, DbName, User, Password);
      return connectionString.ToString();
    }

    private void LoadFromConfigSection(IConfigurationSection section)
    {
      section.Bind(this);
    }

    private void OverrideFromEnvironmentVariables()
    {
      var env = Environment.GetEnvironmentVariables();
      Host = env.Contains("DB_MSSQL_CONNECTION_HOST") ? env["DB_MSSQL_CONNECTION_HOST"].ToString() : Host;
      Port = env.Contains("DB_MSSQL_CONNECTION_PORT") ? env["DB_MSSQL_CONNECTION_PORT"].ToString() : Port;
      DbName = env.Contains("DB_MSSQL_CONNECTION_NAME") ? env["DB_MSSQL_CONNECTION_NAME"].ToString() : DbName;
      User = env.Contains("DB_MSSQL_AUTH_USER") ? env["DB_MSSQL_AUTH_USER"].ToString() : User;
      Password = env.Contains("DB_MSSQL_AUTH_PASSWORD") ? env["DB_MSSQL_AUTH_PASSWORD"].ToString() : Password;
    }
  }
}
