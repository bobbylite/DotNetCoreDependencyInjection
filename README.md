# bobbylite toolkit for .NET Core [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=bobbylite%20.NET%20Core%20toolkit&url=https://github.com/bobbylite/.NETCoreLibrary&hashtags=Inversion-of-Control,Events,Autofac,bobbylite)
bobbylite DotNetCoreDependencyInjection toolkit is a class library that utilizes Microsoft's built in inversion of control dependency injection.

## Run example
There is a test script available to run to see how this repo works. 
Run the following in your terminal.

### Clone the code 
```bash
git clone https://github.com/bobbylite/DotNetCoreDependencyInjection
cd DotNetCoreDependencyInjection
```
### .NET Reference Guide
https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference


### Hot to build
```bash
dotnet build
```

### How to run
```bash
dotnet run --project DotNetCoreDependencyInjection/DotNetCoreDependencyInjection.csproj
```

## How to use in project

### THIS MUST BE UPDATED
Implement your Event so you can have everything you need.  Maybe you want to setup a service for handling an event later?  
```csharp 
using bobbylite.Notifications;

namespace bobbylite {
    public class ApplicationStartedNotification : IAppNotification {

    }
}
```

### Step 2
This is where we will implement our event handler.  We must make sure to inherit the ApplicationHandler<T> class provided. 
```csharp
using System;
using bobbylite.Handlers;

namespace bobbylite {
    public class ApplicationStartedNotificationHandler : ApplicationHandler<ApplicationStartedNotification> {
        protected override void HandleNotification(ApplicationStartedNotification message) {
            Console.WriteLine("Application Started Successfully...");
        }
    }
}
```

### Step 3
```csharp
using System;
using bobbylite.Notifications;
using Autofac;

namespace bobbylite {
    public class ApplicationStartupService : IAutoStart {

        public NotificationManager NotificationManager {get; set;}

        public void Start() => NotificationManager.Notify(new ApplicationStartedNotification());
    }
}
```

### Step 4
Now we need to wire up/connect the ApplicationStartedEvent to the ApplicationStartedEventHandler and the ApplicationStartupService that inherits IAutoStart.
```csharp
using System.Reflection;
using Autofac;
using Module = Autofac.Module;
using bobbylite.Notifications;

namespace bobbylite {
    public class ApplicationModule : Module {
        protected override void Load(ContainerBuilder builder) {
            StartUp(builder);
            StartNotificationManager(builder);
        }

        private void StartUp(ContainerBuilder builder) {
            builder.Register(c => new ApplicationStartupService())
                .As<IAutoStart>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }

        private void StartNotificationManager(ContainerBuilder builder) {
            builder.RegisterType<ApplicationStartedNotificationHandler>()
                .As<IHandleNotifications<ApplicationStartedNotification>>()
                .PropertiesAutowired()
                .SingleInstance();
        }
    }
}
```

### Step 5
Lastly we will build the Autofac container. This is the entry point to the toolkit.
Below is an example of my test code. 
```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using bobbylite;
using bobbylite.DependencyInjection;
using Autofac;

namespace bobbylite
{
    public class UnitTest1
    {
        private IContainer _container;

        [Fact]
        public void Test1()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new bobbylite.DependencyInjection.Modules.CoreModule());
            builder.RegisterModule(new ApplicationModule());
            _container = builder.Build();

            AutoStart();
        }

        private void AutoStart() {
            var resolved = _container.Resolve<ObjectResolver>();
            foreach(var instance in resolved.GetAll<IAutoStart>()) {
                Task.Run(() => {
                    try {
                        instance.Start();
                    } catch (Exception) {
                        // Handle exception
                    }
                });
            }
        }
    }
}
```

## Behind the scenes
Behind the scenes we have two important files that really auto-wire up the notifications to the handlers.
This .NET Core lib uses Autofac's container and interfaces to auto-wire everything in the background. Out of the box
you have NotificationManager because of this Core Module in the lib. 
Take a look below.

#### Core Module
```csharp
using Autofac;
using bobbylite.Notifications;

namespace bobbylite.DependencyInjection.Modules
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ObjectResolver(c.Resolve<IComponentContext>())).AsSelf().SingleInstance();
            builder.RegisterType<NotificationManager>().AsSelf().SingleInstance();
        }
    }
}
```

#### Object Resolver
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace bobbylite.DependencyInjection
{
    public class ObjectResolver
    {
        private readonly IComponentContext _componentContext;

        public ObjectResolver(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public T Get<T>()
        {
            return _componentContext.Resolve<T>();
        }

        public T Get<T>(params Tuple<string, object>[] constructorArgs)
        {
            return _componentContext.Resolve<T>(constructorArgs.Select(arg => new NamedParameter(arg.Item1, arg.Item2)));
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _componentContext.Resolve<IEnumerable<T>>();
        } 
    }
}
```