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