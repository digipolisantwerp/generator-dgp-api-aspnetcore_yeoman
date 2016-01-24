using System;
using System.IO;
using StarterKit.Utilities.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StarterKit
{
    public class ConfigurationConfig
    {
        public ConfigurationConfig(string configPath)
        {
            if ( String.IsNullOrWhiteSpace(configPath) ) throw new ArgumentException(nameof(configPath) + " is null of leeg.", nameof(configPath));
            _configPath = configPath;
        }

        private readonly string _configPath;
        
        public void Configure(IServiceCollection services)
        {
            var logConfigFilename = GetLoggingConfigFileName(); 
            var logConfigSection = CreateConfigurationSection(logConfigFilename);            
            var logConfig = new LoggingConfiguration(logConfigSection);
            services.AddInstance<ILoggingConfiguration>(logConfig);
        }

        private string GetLoggingConfigFileName()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return "macosxloggingconfig.json";
                case PlatformID.Unix:
                    return "linuxloggingconfig.json";
                case PlatformID.Win32NT:
                    return "windowsloggingconfig.json";
                case PlatformID.Win32S:
                    return "windowsloggingconfig.json";
                case PlatformID.Win32Windows:
                    return "windowsloggingconfig.json";
                case PlatformID.WinCE:
                    return "windowsloggingconfig.json";
                case PlatformID.Xbox:
                    return "xboxloggingconfig.json";
                default:
                    throw new ArgumentException(String.Format("OperatingSystem.PlatformId '{0}' is niet gekend.", Environment.OSVersion.Platform));
            }
        }
        
        private IConfiguration CreateConfigurationSection(string pathToJsonFile)
        {
            var path = Path.Combine(_configPath, pathToJsonFile);
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(path);
            var config = builder.Build();
            return config;
        }
    }
}