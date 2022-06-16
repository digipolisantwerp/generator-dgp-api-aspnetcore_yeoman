////using System.Reflection;
////using DbUp;
////using DbUp.Engine;
////using DbUp.Engine.Output;
////using Serilog;
////using StarterKit.DataAccess.Options;

////namespace StarterKit.Startup
////{
////	//Db up script runner, used for PostgresQl
////    public class DbScriptRunner
////    {
////		public static void UpdateDatabase(DataAccessSettingsNpg dataAccessSettings)
////		{
////			if (dataAccessSettings == null || !dataAccessSettings.RunScripts) return;

////			var dbUpdater = DeployChanges.To
////				.PostgresqlDatabase(dataAccessSettings.GetConnectionString())
////				.WithScriptsEmbeddedInAssembly(Assembly.GetEntryAssembly(),
////					s => s.ToLower().StartsWith("starterkit.scripts.0") && !s.ToLower().EndsWith("undo.sql"))
////				.WithTransaction()
////				.LogScriptOutput()
////				.LogTo(new DpUpLogger())
////				.Build();

////			var pendingScripts = dbUpdater.GetScriptsToExecute();
////			if (pendingScripts.Count > 0)
////			{
////				Log.Information("Db update scripts found:");
////				foreach (var script in pendingScripts) Log.Information($"\t- {script.Name}");
////			}
////			else
////			{
////				Log.Information("No update scripts found.");
////			}

////			if (dataAccessSettings.DryRun) return;

////			DatabaseUpgradeResult result = null;
////			result = dataAccessSettings.MarkAllAsExecuted ? dbUpdater.MarkAsExecuted() : dbUpdater.PerformUpgrade();

////			if (!result.Successful) throw result.Error;
////		}

////		private class DpUpLogger : IUpgradeLog
////		{
////			public void WriteError(string format, params object[] args)
////			{
////				Log.Error(format, args);
////			}

////			public void WriteInformation(string format, params object[] args)
////			{
////				Log.Information(format, args);
////			}

////			public void WriteWarning(string format, params object[] args)
////			{
////				Log.Warning(format, args);
////			}
////		}
////	}
////}
