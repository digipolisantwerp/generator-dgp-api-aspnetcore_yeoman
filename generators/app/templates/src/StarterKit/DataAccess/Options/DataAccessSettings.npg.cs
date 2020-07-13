using System;
using Digipolis.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Shared.Constants;

namespace StarterKit.DataAccess.Options
{
  public class DataAccessSettingsNpg
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string DbName { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section)
    {
      services.Configure<DataAccessSettingsNpg>(settings =>
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

    private void LoadFromConfigSection(IConfiguration section)
    {
      section.Bind(this);
    }

    private void OverrideFromEnvironmentVariables()
    {
      var env = Environment.GetEnvironmentVariables();
      Host = env.Contains(DataAccessSettingsConfigKeyNpg.Host) ? env[DataAccessSettingsConfigKeyNpg.Host]?.ToString() : Host;
      Port = env.Contains(DataAccessSettingsConfigKeyNpg.Port) ? env[DataAccessSettingsConfigKeyNpg.Port]?.ToString() : Port;
      DbName = env.Contains(DataAccessSettingsConfigKeyNpg.DbName) ? env[DataAccessSettingsConfigKeyNpg.DbName]?.ToString() : DbName;
      User = env.Contains(DataAccessSettingsConfigKeyNpg.User) ? env[DataAccessSettingsConfigKeyNpg.User]?.ToString() : User;
      Password = env.Contains(DataAccessSettingsConfigKeyNpg.PassWord) ? env[DataAccessSettingsConfigKeyNpg.PassWord]?.ToString() : Password;
    }
  }
}
