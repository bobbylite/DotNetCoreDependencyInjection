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