using System;
using Digipolis.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Shared.Constants;

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
      Host = env.Contains(DataAccessSettingsConfigKeyMs.Host) ? env[DataAccessSettingsConfigKeyMs.Host]?.ToString() : Host;
      Port = env.Contains(DataAccessSettingsConfigKeyMs.Port) ? env[DataAccessSettingsConfigKeyMs.Port]?.ToString() : Port;
      DbName = env.Contains(DataAccessSettingsConfigKeyMs.DbName) ? env[DataAccessSettingsConfigKeyMs.DbName]?.ToString() : DbName;
      User = env.Contains(DataAccessSettingsConfigKeyMs.User) ? env[DataAccessSettingsConfigKeyMs.User]?.ToString() : User;
      Password = env.Contains(DataAccessSettingsConfigKeyMs.PassWord) ? env[DataAccessSettingsConfigKeyMs.PassWord]?.ToString() : Password;
    }
  }
}
