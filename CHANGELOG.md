# generator-dgp-api-aspnetcore

## 9.0.4
- Update nuget packages

## 9.0.2
- Remove obsolete project reference

## 9.0.1
- Remove obsolete nuget package source from nuget.config file to avoid build problems

## 9.0.0
- Updated project to .net 5.0

## 8.0.0
- Updated project to .net core 3.1
- Many nuget updates (deleted DigiPolis.Web reference => outdated)
- Added Api versioning in URL follwoing the Api Guidelines of Digipolis
- Dockerfile adjustments for building, deploying and testing 3.1 apps
- Unit tests examples
- Integration tests: full setup with usage of real db and offcourse rollback
- Added exception handling by default
- Updated swagger
- Updated Yeoman index.js for generating the project
- Fixed es-lint issues
- Update Yeoman generator scripts and npm packages
- Runtime build number (/status/runtime)

## 7.0.0
 - Update to .Net Core 2.2 (target framework .net core 2.2.5)
 - Update nuget packages
 - refactor logging and set logging level from configuration settings
 - facilitate mandatory environment variables
 - run unit tests when building docker image
 - status/runtime endpoint for application process information
 - log application lifecycle events: starting, started, stopped
 - update yeoman generator npm packages
 - replace npm package "guid" with "uuid"

## 6.0.1
 - Update npm packages
 - update yeoman generator script

## 6.0.0
 - Update to .Net Core 2.1 (target framework .net core 2.1.6)
 - Update nuget packages

## 5.3.0
 - Update package Automapper to v7.0.1, Newtonsoft.Json to v11.0.2
 - Update Swashbuckle packages to recent version
 - Update Digipolis toolbox packages to recent version
 - Add package Digipolis.Prometheus for enabling app metrics and added tot Startup.Configure
 - fix in determining logging config from environment variables
 - Added class for setting service agent configuration from environment variables
 - Fixed swagger end-point

## 5.2.0
 - Update Auth toolbox to v3.2.0 (.NET Standard 2.0 compatible)
 - Update DataAccess toolbox to v4.0.0 (.NET Standard 2.0 compatible)

## 5.1.0
 - Working with environment variables instead of .dist files
 - Updates for Docker deployment

## 5.0.3
 - Fix missing using statement and faulty namespaces in test projects

## 5.0.2
 - Missing reference for StaticFiles and TestServer; added AutoMapperTest

## 5.0.1
 - Missing using Microsoft.EntityFrameworkCore.Diagnostics; added for DataAccess

## 5.0.0
 - Update to .Net Core 2.0

## 4.1.0

- added status API.
- added automapper dependency injection & profiles.
- default API versioning enabled.
- default XML Documentation generation enabled.
- update digipolis package dependencies.
- use custom version elastichsearch sink + selflogging enabled.

## 4.0.2

- update version xunit packages and dotnet-xunit CLI tool added to integration test project.

## 4.0.1

- update package versions.

## 4.0.0

- conversion to csproj project files and MSBuild.

## 3.2.0

- ElasticSearch auth headers added.

## 3.1.x

- update of the Digipolis.Serilog packages.

## 3.0.0

- upgrade to .NET Core 1.1.

## 2.3.0

- added IApplicationLogger.

## 2.2.9

- replace 'StarterKit' in dynamically added code. 

## 2.2.8

- use database schema 'main' with NpgSql.

## 2.2.7

- update package versions.

## 2.2.6

- global error handling on by default.

## 2.2.5

- docker elasticsearch tooling added.

## 2.2.4

- removed elasticsearch bufferByteSizeLimit from config.

## 2.2.3

- more serilog elasticsearch settings in config file.

## 2.2.2

- registration of API extensions added in Startup.

## 2.2.1

- DataAccess package version updated.

## 2.2.0

- Logging engine (elastic search) extensions added.

## 2.1.3

- Modified template to support changes to Digipolis.Web toolbox.

## 2.1.2

- Updated toolbox versions.

## 2.1.1

- BUILDNUMBER placeholder in project.json.

## 2.1.0

- generation of data access completed with tooling.

## 2.0.11

- more fixes in db connection string handling.

## 2.0.10

- fix in db connection string handling.

## 2.0.9

- generation of  data access files. 

## 2.0.8

- added appsgroup property to chef config.
- changes needed for hosting the app.

## 2.0.7

- correct generation of SSL port.

## 2.0.6

- update web toolbox version.
- added kestrel port.

## 2.0.5

- added _config to include on publish in project.json.

## 2.0.4

- package version in index.js.

## 2.0.3

- Added version in project.json.
- Removed tools section from project.json.

## 2.0.2

- correction startup method : AddVersionEndpoint.

## 2.0.1

- Digipolis.Web toolbox v2.0.1.

## 2.0.0

- upgrade to .Net Core 1.0 (RTM).

## 1.0.14

- added runtimeinfo page in DEV environment.

## 1.0.13

- added exception handling middleware in Startup.

## 1.0.12

- added version file.

## 1.0.11

- more tests update notifier.

## 1.0.10

- display welcome message first.

## 1.0.9

- update interval to 5 minutes.

## 1.0.8

- test update notifier.

## 1.0.7

- display current version.

## 1.0.6

- no generation when newer version of the generator.

## 1.0.5

- location of package.json.

## 1.0.4

- added update notifier.

## 1.0.3

- casing in file names.

## 1.0.2

- Correlation Toolbox added. 

## 1.0.1

- typo's.

## 1.0.0

- initial version.
