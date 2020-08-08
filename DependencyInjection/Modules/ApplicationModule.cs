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
    }
}