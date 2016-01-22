using Microsoft.Extensions.Configuration;

namespace StarterKit.Utilities.Configs
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public DatabaseConfiguration(IConfiguration config)
		{
			_config = config;
		}
		
		private readonly IConfiguration _config;
        
        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                    _connectionString = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4}",
                                                _config["ConnectionString:Host"], 
                                                _config["ConnectionString:Port"], 
                                                _config["ConnectionString:Name"], 
                                                _config["ConnectionString:Userid"], 
                                                _config["ConnectionString:Password"]);
                return _connectionString;

            }
        }
    }
}