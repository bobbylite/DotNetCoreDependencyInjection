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