# generator-dgp-api-aspnetcore

> Yeoman generator for an ASP.NET 5.0 API project with csproj and MSBuild.

## Installation

Make sure you have installed a recent version of node.js. You can download it here : https://nodejs.org/en/download/current/. 

Install Yeoman :

``` bash
npm install yo -g
```

The _**-g**_ flags install it globally so you can run yeoman from anywhere.

Install the generator :

``` bash
npm install generator-dgp-api-aspnetcore -g
```

## Generate a new ASP.NET Core 5 API project

In a command prompt, navigate to the directory where you want to create the new project and type :

``` bash
yo dgp-api-aspnetcore
```

Answer the questions :-)

If you prefer to skip the prompt and supply the parameters using command line arguments simply add the '--skip-prompt y' argument. 
See the table below for other argument options.

| Parameter        | Options           | Default     |
| ---------------- | ----------------- | ----------- |
| --skip-prompt    | y/n               | n           |
| --delete-content | y/n               | y           |
| --name           | project name text | Starter app |
| --database       | mo/ms/p/n         | p           |
| --http-kestrel   | port number       |             |
| --http-iis       | port number       |             |
| --https-iis      | port number       |             |

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

The Swagger-UI starts when browsing to the launched application base path, ex. https://localhost:44300 redirects to https://localhost:44300/swagger/index.html.
A link to the automatically generated json swagger definition based on the implementation within the application can be found on this UI.

### Logging

Everything is preset for logging to console via Serilog (from generator v10.0.0) to comply to the Digipolis logging requirements. 

To apply these changes in existing projects, follow the instructions listed [here](https://github.com/digipolisantwerp/generator-dgp-api-aspnetcore_yeoman/blob/master/loggingchanges.md).

### Known issues after generation

None.

## Contributing

Pull requests are always welcome, however keep the following things in mind:

- New features (both breaking and non-breaking) should always be discussed with the [repo's owner](#support). If possible, please open an issue first to discuss what you would like to change.
- Fork this repo and issue your fix or new feature via a pull request.
- Please make sure to update tests as appropriate. Also check possible linting errors and update the CHANGELOG if applicable.

## Support

Erik Seynaeve (<erik.seynaeve@digipolis.be>)
