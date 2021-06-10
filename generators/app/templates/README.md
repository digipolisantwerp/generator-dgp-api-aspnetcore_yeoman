# StarterKit 

Here comes the information about my project.

## Initial build and deploy

- In order to deploy your application to a Digipolis Openshift container it's necessary to add following section to the AppConfig of
  your application:
  "server": {
      "urls": "http://*:80"
  }

## Integration tests

- The integration tests of this project will execute automatically during the build process (see Dockerfile).
- Important notice: these tests make use of the real life database with a rollback feature. Please be aware that calling non-mocked services can cause unwanted side effects. Please contact one of the contributers for more info. 

## Logging

Log messages are printed to the console and formatted in JSON according to the [Digipolis logging guidelines](https://github.com/digipolisantwerpdocumentation/logging-requirements).

Incoming and outgoing API calls are logged by default. Sensitive headers such as `Authorization` and `ApiKey` are not logged. You can override the default headers and other settings in [AppSettings](src/StarterKit/Shared/Options/AppSettings.cs) by using the configuration files or environment variables.

See 
[src/StarterKit/Framework/Logging](src/StarterKit/Framework/Logging) for more details.
