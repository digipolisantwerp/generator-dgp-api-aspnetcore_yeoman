using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared;
using StarterKit.Shared.Constants;

namespace StarterKit.DataAccess.Options
{
  /// <summary>
  /// These are the dataaccess settings for Entity Framework PostgreSQL.
  /// When PostgreSQL is chosen as the DB provider this file will be used
  /// </summary>
  public class DataAccessSettingsNpg : SettingsBase
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string DbName { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section,
      IHostEnvironment environment)
    {
      services.Configure<DataAccessSettingsNpg>(settings =>
      {
        settings.LoadFromConfigSection(section);
        settings.OverrideFromEnvironmentVariables(environment);
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

    private void OverrideFromEnvironmentVariables(IHostEnvironment environment)
    {
      Host = GetValue(Host, DataAccessSettingsConfigKeyNpg.Host, environment);
      Port = GetValue(Port, DataAccessSettingsConfigKeyNpg.Port, environment);
      DbName = GetValue(DbName, DataAccessSettingsConfigKeyNpg.DbName, environment);
      User = GetValue(User, DataAccessSettingsConfigKeyNpg.User, environment);
      Password = GetValue(Password, DataAccessSettingsConfigKeyNpg.PassWord, environment);
    }
  }
}
