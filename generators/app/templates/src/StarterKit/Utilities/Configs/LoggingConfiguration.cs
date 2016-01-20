using Microsoft.Extensions.Configuration;

namespace StarterKit.Utilities.Configs
{
    public class LoggingConfiguration : ILoggingConfiguration
    {
        public LoggingConfiguration(IConfiguration config)
		{
			_config = config;
		}
		
		private readonly IConfiguration _config;
        
        private string _name;
        public string Name
        {
            get
            {
                if ( _name == null )
                    _name = _config["Name"];
                return _name;
            }
        }

        private LoggingFileTargetConfiguration _fileTarget;
        public LoggingFileTargetConfiguration FileTarget
        {
            get
            {
                if ( _fileTarget == null )
                {
                    _fileTarget = new LoggingFileTargetConfiguration()
                    {
                        FileName = _config["FileTarget:FileName"],
                        Layout = _config["FileTarget:Layout"],
                        Level = _config["FileTarget:Level"],
                        Name = _config["FileTarget:Name"],
						Path = _config["FileTarget:Path"]
                    };
                }
                return _fileTarget;
            }
        }
    }
}