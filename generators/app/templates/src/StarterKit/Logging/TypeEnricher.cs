using Serilog.Core;
using Serilog.Events;

namespace StarterKit.Logging
{
    public class TypeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var type = string.Empty;

            switch (logEvent.Exception)
            {
                case null:
                    type = "application";
                    break;
                default:
                    type = "technical";
                    break;
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "Type", type));
        }
    }
}
