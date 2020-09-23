using System;
using Digipolis.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared;
using StarterKit.Shared.Constants;

namespace StarterKit.DataAccess.Options
{
  public class DataAccessSettingsMs: SettingsBase
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string DbName { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public static void RegisterConfiguration(IServiceCollection services, IConfigurationSection section, IHostEnvironment environment)
    {
      services.Configure<DataAccessSettingsMs>(settings =>
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

    private void LoadFromConfigSection(IConfigurationSection section)
    {
      section.Bind(this);
    }

    private void OverrideFromEnvironmentVariables(IHostEnvironment environment)
    {
      Host = GetValue(Host, DataAccessSettingsConfigKeyMs.Host, environment);
      Port = GetValue(Port, DataAccessSettingsConfigKeyMs.Port, environment);
      DbName = GetValue(DbName, DataAccessSettingsConfigKeyMs.DbName, environment);
      User = GetValue(User, DataAccessSettingsConfigKeyMs.User, environment);
      Password = GetValue(Password, DataAccessSettingsConfigKeyMs.PassWord, environment);
    }
  }
}
