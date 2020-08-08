# bobbylite Decoupled Dependency Injection Application [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=bobbylite%20.NET%20Core%20toolkit&url=https://github.com/bobbylite/DotNetCoreDependencyInjection&hashtags=Inversion-of-Control,Events,bobbylite)
bobbylite DotNetCoreDependencyInjection is a dependency injection application scaffold that utilizes Microsoft's built in inversion of control dependency injection to decouple registered services, events, and handlers.

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

### Step 1 - Implement Service
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

### Step 2 - Register Service
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

### Step 3 - Implment Notifications
Notifications will be broken into two steps. The Notification itself, which can be used anywhere in the application and then the actual notification handler.  You get to tell the handler how to handle the notification object and you get to tell the notification object what is contained inside of it. 
 #### Creating a notification
 Let's create a simple notification that contains a string. 
 ```csharp
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.Notifications
{
    public class ApplicationStartedNotification : INotification
    {
        public string ApllicationStartedMessage {get; set;}
    }
}
 ```
#### Creating a notification handler
Let's create a handler that receives the notification, grabs the string and logs it. We must use the BaseNotificationHanlder<T> interface to allow us to handle notifications on the back end later on. 
```csharp
using DependencyInjectionApp.Notifications;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionApp.NotificationHandlers
{
    public class ApplicationStartedNotificationHandler : BaseNotificationHandler<ApplicationStartedNotification>
    {
        public readonly ILogger<ApplicationStartedNotificationHandler> _logger;
        
        public ApplicationStartedNotificationHandler(ILogger<ApplicationStartedNotificationHandler> logger)
        {
            _logger = logger;
        }
        protected override void HandleNotification(ApplicationStartedNotification notification)
        {
            _logger.LogInformation($"MESSAGE: {notification.ApllicationStartedMessage}");
        }
    }
}
```

#### Registering the notification handler to the notification event
In the application dependecy injection module we can register the notification handler to be explicitly associated with our newly created notification. Registering with AddTransient<interface, class>() will instantiate a new instance everytime it is resolved.
```csharp
using DependencyInjectionApp.Services;
using Microsoft.Extensions.DependencyInjection;
using DependencyInjectionApp.NotificationHandlers;
using DependencyInjectionApp.Notifications;
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.DependencyInjection.Modules
{
    public class ApplicationModule : BaseModule 
    {
        protected override void RegisterServiceModule(IServiceCollection serviceModule)
        {
            // Register application handlers
            serviceModule.AddTransient<IHandleNotifications<ApplicationStartedNotification>, ApplicationStartedNotificationHandler>();

            // Register Notification Manager 
            serviceModule.AddSingleton<INotificationManager, NotificationManager>();
            
            // Startup application
            serviceModule.AddHostedService<AppStartupService>();
        }
```

#### Using our new notification
We can call our notification handler from any class that's already registered. In this case, let's use our Web Endpoint class.
We will send a start and end notification to our handler. Earlier we decided that our handler will just print the notification string.
```csharp
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.Common;
using DependencyInjectionApp.Notifications;

namespace DependencyInjectionApp.Services 
{
    public class WebEndpointService : IAutoStart
    {
        public readonly ILogger<WebEndpointService> _logger;
        public readonly INotificationManager _notificationManager;

        public WebEndpointService(INotificationManager notificationManager, ILogger<WebEndpointService> logger)
        { 
            _logger = logger;
            _notificationManager = notificationManager;
        }
        
        public void Start() 
        {
            _notificationManager.Notify(new ApplicationStartedNotification{
                ApllicationStartedMessage = "Endpoint Started Notification"
            });
        }

        public void Stop()
        {
            _notificationManager.Notify(new ApplicationStartedNotification{
                ApllicationStartedMessage = "Endpoint Stopped Notification"
            });
        }
    }
}
```

## Behind the scenes
Behind the scenes we have two important files that really auto-wire up the notifications to the handlers.
This .NET Core lib uses Autofac's container and interfaces to auto-wire everything in the background. Out of the box
you have NotificationManager because of this Core Module in the lib. 
Take a look below.

### Notifications
Let's take a look at how notifications are decoupled on the back end. 

#### Base notification handler 
When creating a new notification handler we have to inherit the IHandleNotifications interface. This will allow us to later use the service provider to grab the services registered with an associated type. 
```csharp
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.NotificationHandlers 
{
    public abstract class BaseNotificationHandler<T> : IHandleNotifications<T> where T : INotification
    {
        public void Handle(T notification)
        {
            try
            {
                HandleNotification(notification);
            } 
            catch
            {
                // Handle error
            }
        }

        protected abstract void HandleNotification(T notification);
    }
}
```
#### Notification manager
The notification manager's job is to use the service provider to find which handler you are trying to notify, and call that handlers handle() method. 
```csharp
using DependencyInjectionApp.Common;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.DependencyInjection
{
    public class NotificationManager : INotificationManager
    {
        public readonly IServiceProvider _serviceProvider;

        public NotificationManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Notify<T>(T notification) where T : INotification
        {
            HanldeNotification(_serviceProvider, notification);
        }

        private static void HanldeNotification<T>(IServiceProvider serviceProvider, T notification) where T : INotification
        {
            var registeredNotificationHandlers = serviceProvider.GetServices<IHandleNotifications<T>>();
            foreach (var handler in registeredNotificationHandlers)
            {
                handler.Handle(notification);
            }
        }
    }
}
```

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