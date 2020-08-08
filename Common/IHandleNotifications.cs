namespace DependencyInjectionApp.Common
{
    public interface IHandleNotifications<in T> where T : INotification
    {
        void Handle(T notification);
    }
}