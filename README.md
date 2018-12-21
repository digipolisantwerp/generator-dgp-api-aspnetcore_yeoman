# generator-dgp-api-aspnetcore

> Yeoman generator for an ASP.NET Core 2.1 API project with csproj and MSBuild.

## Installation

Make sure you have installed a recent version of node.js. You can download it here : https://nodejs.org/en/. 

Install Yeoman :

``` bash
npm install yo -g
``` 

The _**-g**_ flags install it globally so you can run yeoman from anywhere.

Install the generator :

``` bash
npm install generator-dgp-api-aspnetcore -g
```

## Generate a new ASP.NET Core 2.0 API project

In a command prompt, navigate to the directory where you want to create the new project and type :

``` bash
yo dgp-api-aspnetcore
```

Answer the questions :-)

## The ASP.NET Core solution

### Startup

Enter your application Id, which you can find in AppConfig, in _config\app.json. It will be used in the StartUp class -> ConfigureServices -> services.AddApplicationServices
...to do...

### DependencyRegistration

The DependencyRegistration class is used to register the dependencies of the application, so that is uses the built-in dependency injection framework.

``` csharp 
services.AddTransient<IMyBusinessClass, MyBusinessClass>();

services.AddScoped<IMyBusinessClass, MyBusinessClass>();

services.AddSingleton<IMyBusinessClass, MyBusinessClass>();
```  
**Transient**
Every time the service is injected, a new one is instantiated.  

**Scoped**
The lifetime of the service is tied to the request scope. Only 1 instance is instantiated per request.  

**Singleton**
Only one instance is instantiated for the whole application.  

More info about the dependency injection in ASP.NET Core can be found at : https://docs.asp.net/en/latest/fundamentals/dependency-injection.html. 

### AutoMapperRegistration

Use this class to register your AutoMapper mappings.

More info can be found at : http://automapper.org/.

### Swagger

...to do...

### Logging

Almost everything is preset for logging to Kibana. Enter the name of your logging index in _config\logging.json -> "indexFormat": "logstash-{tenant}-{your logging index goes here}-{0:yyyy.MM.dd}" .
The maximum length of tenant (application- or system-) and logging index name is 30 characters !


### Known issues after generation

Project has 2 warnings of type NU1608 after generation. The cause of this is an outdated package reference by Serilog.Sinks.ElasticSearch to 
NewtonSoft.Json version between 9.0.0 and 10.0.0. Be mindful of possible bugs because of this!
