using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Shared;
using StarterKit.Shared.Constants;
using StarterKit.Shared.Options;
using System;
using System.Collections.Generic;

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
		public bool RunScripts { get; set; }
		public bool MarkAllAsExecuted { get; set; }
		public bool DryRun { get; set; }
		

		public string GetConnectionString()
		{
			ushort port = 0;
			try
			{
				port = ushort.Parse(Port);
			}
			catch (InvalidOperationException ex)
			{
				throw new InvalidOperationException("Database port must be a number from 0 to 65536.",
					ex.InnerException ?? ex);
			}

			var connectionString = new ConnectionString(ConnectionType.PostgreSQL, Host, port, DbName, User, Password);
			return connectionString.ToString();
		}

		public static IConfigurationSection GetConfigurationSection(IConfiguration Configuration)
		{
			return Configuration.GetSection(ConfigurationSectionKey.DataAccess).GetSection(ConfigurationSectionKey.ConnectionString);
		}

		public static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
		{
			// dataAccess settings from json are already overridden by environment variables in Program.cs
			services.Configure<DataAccessSettingsNpg>(GetConfigurationSection(configuration));
		}
	}

	public static class DataAccessExtensionsNpg
	{

		/// <summary>
		/// load appsettings from json and overwrite necessary params from environment variables
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="hostingEnv"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDataAccessConfiguration(this IConfigurationBuilder configurationBuilder,
																	   IHostEnvironment hostingEnv)
		{
			// load in this order so that json-settings will be overridden with environment settings when getting the configuration section;
			configurationBuilder.AddJsonFile(JsonFilesKey.DataAccessJson);
			configurationBuilder.AddInMemoryCollection(GetEnvironmentVariablesDict(hostingEnv));

			return configurationBuilder;
		}

		private static Dictionary<string, string> GetEnvironmentVariablesDict(IHostEnvironment hostingEnv)
		{
			var environmentDict = new Dictionary<string, string>();

			// overwrite settings from environment variables;
			// if overwriting the json-file value from environment variables isn't necesarry, omit the variable declaration in appconfig and beneath
			if (hostingEnv.EnvironmentName != RuntimeEnvironment.Local)
			{
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.Host, "DataAccess:ConnectionString:Host", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.Port, "DataAccess:ConnectionString:Port", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.DbName, "DataAccess:ConnectionString:DbName", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.User, "DataAccess:ConnectionString:User", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.PassWord, "DataAccess:ConnectionString:Password", environmentDict);

				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.DryRun, "DataAccess:ConnectionString:DryRun", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.MarkAllAsExecuted, "DataAccess:ConnectionString:MarkAllAsExecuted", environmentDict);
				ConfigUtil.FillFromEnvironment(DataAccessSettingsConfigKeyNpg.RunScripts, "DataAccess:ConnectionString:RunScripts", environmentDict);
			}

			return environmentDict;
		}
	}
}
