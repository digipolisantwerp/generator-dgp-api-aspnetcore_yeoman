'use strict';
var yeoman = require('yeoman-generator');
var chalk = require('chalk');
var yosay = require('yosay');
var del = require('del');
var nd = require('node-dir');
var Guid = require('guid');
var updateNotifier = require('update-notifier');
var pkg = require('./../../package.json');
var path = require('path');

module.exports = yeoman.generators.Base.extend({
  
  prompting: function () {
    var done = this.async();

    // greet the user
    this.log(yosay('Welcome to the fantastic Yeoman ' + chalk.green('dgp-api-aspnetcore') + ' ' + chalk.blue('(' + pkg.version + ')') + ' generator!'));
    
    var notifier = updateNotifier({
        pkg,
        updateCheckInterval: 1000 * 60 * 5      // check every 5 minutes. 
    });
    notifier.notify();
    if (notifier.update != undefined) return;
    
    // ask project parameters
    var prompts = [{
      type: 'input',
      name: 'deleteContent',
      message: 'Delete the contents of this directory before generation (.git will be preserved) ? (y/n):',
      default: 'y'
    },
    {
      type: 'input',
      name: 'projectName',
      message: "Enter the name of the new project (don't forget the Pascal-casing):"
    }, 
    {
      type: 'input',
      name: 'kestrelHttpPort',
      message: 'Enter the HTTP port for the kestrel server:'
    }, 
    {
      type: 'input',
      name: 'iisHttpPort',
      message: 'Enter the HTTP port for the IIS Express server:'
    },
    {
      type: 'input',
      name: 'iisHttpsPort',
      message: 'Enter the HTTPS port for the IIS Express server:'
    },
    {
      type: 'input',
      name: 'dataProvider',
      message: 'Will you be using Entity Framework with MSSQL, PostgreSQL or Not ? (m/p/n):',
      default: 'p'
    }];

    this.prompt(prompts, function (props) {
      this.props = props;     // To access props later use this.props.someOption;
      done();
    }.bind(this));
  },

  writing: function () {
  
    // empty target directory
    console.log('Emptying target directory...');
    if ( this.props.deleteContent == 'y' ) {
        del.sync(['**/*', '!.git', '!.git/**/*'], { force: true, dot: true });
    }
    
    var projectName = this.props.projectName;
    var lowerProjectName = projectName.toLowerCase(); 
    
    var solutionItemsGuid = Guid.create();
    var srcGuid = Guid.create();
    var testGuid = Guid.create();
    var starterKitGuid = Guid.create();
    var integrationGuid = Guid.create();
    var unitGuid = Guid.create();
    
    var kestrelHttpPort = this.props.kestrelHttpPort;
    var iisHttpPort = this.props.iisHttpPort;
    var iisHttpsPort = this.props.iisHttpsPort;
    var dataProvider = getDataProvider(this.props.dataProvider);
    
    var copyOptions = { 
      process: function(contents) {
        var str = contents.toString();
        var result = str.replace(/StarterKit/g, projectName)
                        .replace(/starterkit/g, lowerProjectName)
                        .replace(/C3E0690A-0044-402C-90D2-2DC0FF14980F/g, solutionItemsGuid.value.toUpperCase())
                        .replace(/05A3A5CE-4659-4E00-A4BB-4129AEBEE7D0/g, srcGuid.value.toUpperCase())
                        .replace(/079636FA-0D93-4251-921A-013355153BF5/g, testGuid.value.toUpperCase())
                        .replace(/BD79C050-331F-4733-87DE-F650976253B5/g, starterKitGuid.value.toUpperCase())
                        .replace(/948E75FD-C478-4001-AFBE-4D87181E1BEC/g, integrationGuid.value.toUpperCase())
                        .replace(/0A3016FD-A06C-4AA1-A843-DEA6A2F01696/g, unitGuid.value.toUpperCase())
                        .replace(/http:\/\/localhost:51002/g, "http://localhost:" + kestrelHttpPort)
                        .replace(/http:\/\/localhost:51001/g, "http://localhost:" + iisHttpPort)
                        .replace(/"sslPort": 44300/g, "\"sslPort\": " + iisHttpsPort)
                        .replace(/\/\/--dataaccess-package--/g, dataProvider.package)
                        .replace(/\/\/--dataaccess-startupImports--/g, dataProvider.startupImports)
                        .replace(/\/\/--dataaccess-startupServices--/g, dataProvider.startupServices)
                        .replace(/\.AddJsonFile\("app\.json"\)/g, dataProvider.startupCtor)
                        .replace(/\/\/--dataaccess-connString--/g, dataProvider.connString)
                        .replace(/\/\/--dataaccess-tools--/g, dataProvider.tools);
        return result;
      }
    };
     
     var source = this.sourceRoot();
     var dest = this.destinationRoot();
     var fs = this.fs;
     
     // copy files and rename starterkit to projectName
    
     console.log('Creating project skeleton...');

     nd.files(source, function (err, files) {
      for ( var i = 0; i < files.length; i++ ) {
        var filename = files[i].replace(/StarterKit/g, projectName)
                               .replace(/starterkit/g, lowerProjectName)
                               .replace('.npmignore', '.gitignore')
                               .replace('dataaccess.ms.json', 'dataaccess.json')
                               .replace('dataaccess.npg.json', 'dataaccess.json')
                               .replace(source, dest);
        switch (dataProvider.input)
        {
          case 'p':
            if (files[i].indexOf('dataaccess.ms.json') > -1 ||
                files[i].indexOf('dataaccess.ms.json.dist') > -1 ) {
              console.log(files[i] + ' not created');
            } else {
              fs.copy(files[i], filename, copyOptions);
            }
            break;
          case 'm':
            if (files[i].indexOf('dataaccess.npg.json') > -1 ||
                files[i].indexOf('dataaccess.npg.json.dist') > -1 ) {
              console.log(files[i] + ' not created');
            } else {
              fs.copy(files[i], filename, copyOptions);
            }
            break;
          default:
            if (files[i].indexOf('EntityContext.cs') > -1 ||
                files[i].indexOf('dataaccess.ms.json.dist') > -1 ||
                files[i].indexOf('dataaccess.npg.json.dist') > -1 ||
                files[i].indexOf('dataaccess.ms.json') > -1 || 
                files[i].indexOf('dataaccess.npg.json') > -1 ) {
              console.log(files[i] + ' not created');
            }
            else {
              fs.copy(files[i], filename, copyOptions);            
            }
        }
      }
    });

  },

  install: function () {
    // this.installDependencies();
    //this.log('----');
  }
});

function getDataProvider(input) {
  var efCorePackage = '"Microsoft.EntityFrameworkCore": "1.0.0",\n';
  var efDesignPackage = '        "Microsoft.EntityFrameworkCore.Design": "1.0.0-preview2-final",'
  var npgSqlPackage = '        "Npgsql.EntityFrameworkCore.PostgreSQL": "1.0.1",\n';
  var sqlServerPackage = '        "Microsoft.EntityFrameworkCore.SqlServer": "1.0.0",\n';
  var dataAccessPackage = '        "Digipolis.DataAccess": "2.5.2",';
  var usings = 'using Microsoft.EntityFrameworkCore;\nusing Microsoft.EntityFrameworkCore.Infrastructure;\nusing Digipolis.DataAccess;';
  var ctor = '.AddJsonFile("app.json")\n                .AddJsonFile("dataaccess.json")';
  var tools = '"Microsoft.EntityFrameworkCore.Tools": { "version": "1.0.0-preview2-final", "type": "build" },';

  var dataProvider = { input: input, package: '', startupServices: '', startupImports: '', startupCtor: '.AddJsonFile("app.json")', connString: '', tools: '' };

  if (input.toLowerCase() === 'p') {
      dataProvider.package = efCorePackage + efDesignPackage + npgSqlPackage + dataAccessPackage;
      dataProvider.startupServices = 'services.AddDataAccess<EntityContext>();\n' +
                                     '            var connString = GetConnectionString();\n' +
                                     '            services.AddDbContext<EntityContext>(options => {\n' +
                                     '                options.UseNpgsql(connString);\n' +
                                     '                options.ConfigureWarnings(config => config.Throw(RelationalEventId.QueryClientEvaluationWarning));\n' +
                                     '            });';
      dataProvider.startupImports = usings;
      dataProvider.startupCtor = ctor;
      dataProvider.connString = getConnectionString();
      dataProvider.tools = tools;
  }
  else if (input.toLowerCase() === 'm') {
      dataProvider.package = efCorePackage + efDesignPackage + sqlServerPackage + dataAccessPackage;
      dataProvider.startupServices = 'services.AddDataAccess<EntityContext>();\n' +
                                     '            var connString = GetConnectionString();\n' +
                                     '            services.AddDbContext<EntityContext>(options => {\n' +
                                     '                options.UseSqlServer(connString);\n' +
                                     '                options.ConfigureWarnings(config => config.Throw(RelationalEventId.QueryClientEvaluationWarning));\n' +
                                     '            });';
      dataProvider.startupImports = usings;
      dataProvider.startupCtor = ctor;
      dataProvider.connString = getConnectionString();
      dataProvider.tools = tools;
  };

  return dataProvider;
}

function getConnectionString()
{
   var code = 'private string GetConnectionString()\n' +
          '        {\n' +
          '            var configSection = Configuration.GetSection("DataAccess").GetSection("ConnectionString");\n\n' +
          '            var host = configSection.GetValue<string>("Host");\n' +
          '            var dbname = configSection.GetValue<string>("DbName");\n' +
          '            var user = configSection.GetValue<string>("User");\n' +
          '            var password = configSection.GetValue<string>("Password");\n\n' +
          '            ushort port = 0;\n' +
          '            try\n' +
          '            {\n' + 
          '                port = configSection.GetValue<ushort>("Port");\n' +
          '            }\n' +
          '            catch (InvalidOperationException ex)\n' +
          '            {\n' +
          '                throw new InvalidOperationException("Database port must be a number from 0 to 65536.", ex.InnerException ?? ex);\n' +
          '            }\n\n' +
          '            var connectionString = new ConnectionString(host, port, dbname, user, password);\n' +
          '            return connectionString.ToString();\n' +
          '        }\n';

   return code;
}

