using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

namespace StarterKit.Framework.Logging
{
	public class TypeEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var types = new List<string>();

			switch (logEvent.Exception)
			{
				case null:
					types.Add("application");
					break;
				default:
					types.Add("technical");
					break;
			}

			logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
				"Type", types));
		}
	}
}
