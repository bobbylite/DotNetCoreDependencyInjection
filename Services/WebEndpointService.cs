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