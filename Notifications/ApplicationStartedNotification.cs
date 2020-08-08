using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.Notifications
{
    public class ApplicationStartedNotification : INotification
    {
        public string ApllicationStartedMessage {get; set;}
    }
}