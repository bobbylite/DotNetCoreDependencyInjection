namespace DependencyInjectionApp.Common
{
    public interface INotificationManager
    {
        void Notify<T>(T message) where T : INotification;
    }
}