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