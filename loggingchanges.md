# code changes to comply with logging requirements

## copy Logging logic to your own application:

	Code can be found [here](https://github.com/digipolisantwerp/generator-dgp-api-aspnetcore_yeoman/tree/master/generators/app/templates/src/StarterKit):
	* StarterKit\Framework\Logging
	* StarterKit\Shared\Options\Logging
	* StarterKit\Shared\Constants\LogSettingsEnvVariableKey.cs
	* StarterKit\Framework\Extensions\StringExtensions.cs (ToCamelCase)
	
## copy configuration keys below from StarterKit\Shared\Constants\ConfigurationSectionKey.cs
  
	public const string LogSettings = "LogSettings";
    public const string Serilog = "Serilog";
    public const string SerilogDev = "SerilogDev";
	
## modify _config\logging.json to new structure:

	Section "LogSettings" for initializing class Options.Logging.LogSettings
	Section "Serilog" with default Serilog configuration
	Section "SeriLogDev" with Serilog configuration for environment "Development" (can be changed in LoggingExtension.UseLogging)
	
	Replace StarterKit" with the own application name for properties "SeriLog/Dev.WriteTo.formatter"

	An example json-object for appConfig can be found at the bottom of class
	generator-dgp-api-aspnetcore_yeoman\generators\app\templates\src\StarterKit\Shared\Constants\LogSettingsEnvVariableKey.cs

## add/upgrade nuget packages. These packages target framework net5.0. Upgrade your project first if it uses an older framework version!

	<PackageReference Include="Digipolis.Serilog" Version="5.0.0" />
    <PackageReference Include="Digipolis.Serilog.AuthService" Version="6.0.0" />
    <PackageReference Include="Digipolis.Serilog.Correlation" Version="5.0.0" />

## Program.cs:

	``` csharp 
	WebHost.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration((hostingContext, config) =>
			{
				...
				config.AddLoggingConfiguration(env);
				...
			})
		.UseSeriLog()
	```

	Configuration via "ConfigureLogging" can  be removed because logging to Console is handled via Serilog (cfr. UseSeriLog() statement).

## Startup
	ConfigureServices: 
		Add call to "services.AddLogging(Configuration, Environment);". 
		This also loads the LogSettings Options class
		
	Configure:
		Add call to "loggerFactory.UseLogging(app, appLifetime, Configuration, Environment);" at beginning of function
		Add Serilog self loggin:
			Serilog.Debugging.SelfLog.Enable(Console.Out);
		
		Activate middleware for logging incoming requests. During runtime this can be (un)enabled via configuration (see logging.json)
			app.UseMiddleware<IncomingRequestLogger>();

## LaunchSettings.json:
	eventueel logging EnvironmentVariables overnemen voor lokale testen
	generator-dgp-api-aspnetcore_yeoman\generators\app\templates\src\StarterKit\Properties\launchSettings.json
	
## Use dedicated DelegatingHandler to add outgoing request logging for ServiceAgents
	
	StarterKit\Framework\Logging\DelegatingHandler\OutgoingRequestLogger.cs
	
	``` csharp 
	
	services.AddHttpClient(nameof(myServiceAgent), (provider, client) =>
            {                ...            })
            ...
            .AddHttpMessageHandler<MyProject.Framework.Logging.OutgoingRequestLogger<MyServiceAgent>>()
	```
	
## add logging where needed in your application by injecting "ILogger<MyClass> logger" in the constructor of the desired classes

