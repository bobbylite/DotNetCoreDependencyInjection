# bobbylite toolkit for .NET Core [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=bobbylite%20.NET%20Core%20toolkit&url=https://github.com/bobbylite/DotNetCoreDependencyInjection&hashtags=Inversion-of-Control,Events,bobbylite)
bobbylite DotNetCoreDependencyInjection is a dependency injection application scaffold that utilizes Microsoft's built in inversion of control dependency injection to register services, events, and handlers.

## Run example
Run the following in your terminal.

### Clone the code 
```bash
git clone https://github.com/bobbylite/DotNetCoreDependencyInjection
cd DotNetCoreDependencyInjection
```
### .NET Reference Guide
To add as a library reference you can refer to this documentation.
https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference


### Hot to build
```bash
dotnet build
```

### How to run
```bash
dotnet run --project DotNetCoreDependencyInjection.csproj
```

## How to use in project

### THIS MUST BE UPDATED
Let's setup a simple WebEndpoint Service. We want this to start automatically when the app runs and stop automatically when the app stops... So we can implement the IAutoStart interface provided. 
```csharp 
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.Services 
{
    public class WebEndpointService : IAutoStart
    {
        public readonly ILogger<WebEndpointService> _logger;

        public WebEndpointService(ILogger<WebEndpointService> logger)
        { 
            _logger = logger;
        }
        
        public void Start() 
        {
            _logger.LogInformation("Web Endpoint Started!");
        }

        public void Stop()
        {
            _logger.LogInformation("Web Endpoint Stopped.");
        }
    }
}
```

### Step 2
We must then let the dependency injection container know that the service is available. We do this by registering the class in the ServicesModule class. Registering with AddSinglton<interface, class>() will instantiate one instance that the container manages. 
```csharp
using DependencyInjectionApp.Services;
using DependencyInjectionApp.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.DependencyInjection
{
    public class ServicesModule : BaseModule 
    {
        protected override void RegisterServiceModule(IServiceCollection serviceModule)
        {
            serviceModule.AddSingleton<IAutoStart, WebEndpointService>();
        }
    }
}
```

## Behind the scenes
Behind the scenes we have two important files that really auto-wire up the notifications to the handlers.
This .NET Core lib uses Autofac's container and interfaces to auto-wire everything in the background. Out of the box
you have NotificationManager because of this Core Module in the lib. 
Take a look below.

#### Base Module
```csharp
using Microsoft.Extensions.DependencyInjection;
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.DependencyInjection
{
    public abstract class BaseModule : IBaseModule 
    {
        public void BuildModule(IServiceCollection service)
        {
            try 
            {
                RegisterServiceModule(service);
            } catch
            {
                // Handle 
            }
        }

        protected abstract void RegisterServiceModule(IServiceCollection serviceModule);
    }
}
```

#### Application Startup Service
This is the service that identifies objects that are registered as auto start classes. 
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.Common;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace DependencyInjectionApp.Services
{
    public class AppStartupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppStartupService> _logger;

        public AppStartupService(IServiceProvider provider, ILogger<AppStartupService> logger)
        {
            _serviceProvider = provider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("Starting application...");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.StackTrace);
                }
            }, cancellationToken);

            return AutoHandleTask(ServiceActions.Start);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Stopping application...");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.StackTrace);
            }

            return AutoHandleTask(ServiceActions.Stop);
        }

        private Task AutoHandleTask(ServiceActions action)
        {
            var registeredAutoStartServices = _serviceProvider.GetServices<IAutoStart>();
            foreach (var registeredService in registeredAutoStartServices)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (action.Equals(ServiceActions.Start)) registeredService.Start();
                        if (action.Equals(ServiceActions.Stop)) registeredService.Stop();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.StackTrace);
                    }
                });
            }

            return Task.CompletedTask;
        }

        private enum ServiceActions
        {
            Start,
            Stop
        }
    }
}
```

#### Application Startup Registration
This is the entry point for our auto start objects.
```csharp
using DependencyInjectionApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.DependencyInjection
{
    public class ApplicationModule : BaseModule 
    {
        protected override void RegisterServiceModule(IServiceCollection serviceModule)
        {
            serviceModule.AddHostedService<AppStartupService>();
        }
    }
}
```

### App
This is the entry point for the application. 
```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.DependencyInjection;

namespace DependencyInjectionApp
{
    public class App
    {
        public static void Main(string[] args) => HostBuilder(args).Build().Run();
        public static IHostBuilder HostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging=>
                {
                    logging.AddConsole();
                    logging.AddFile("./dependencyInjectionApp/logs/AppLog.log");
                })
                .ConfigureAppConfiguration((hostingContext, config)
                    => config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false))
                .ConfigureServices((services) => new ApplicationModule().BuildModule(services))
                .ConfigureServices((services) => new ServicesModule().BuildModule(services));
    }
}

```