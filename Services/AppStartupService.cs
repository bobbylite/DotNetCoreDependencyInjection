using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.Common;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace DependencyInjectionApp.Services
{
    public class AppStartupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppStartupService> _logger;

        public AppStartupService(IServiceProvider provider, ILogger<AppStartupService> logger)
        {
            _serviceProvider = provider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("Starting application...");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.StackTrace);
                }
            }, cancellationToken);

            return AutoHandleTask(ServiceActions.Start);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Stopping application...");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.StackTrace);
            }

            return AutoHandleTask(ServiceActions.Stop);
        }

        private Task AutoHandleTask(ServiceActions action)
        {
            var registeredAutoStartServices = _serviceProvider.GetServices<IAutoStart>();
            foreach (var registeredService in registeredAutoStartServices)
            {
                Task.Run(() =>
                {
                    try
                    {
                        if (action.Equals(ServiceActions.Start)) registeredService.Start();
                        if (action.Equals(ServiceActions.Stop)) registeredService.Stop();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.StackTrace);
                    }
                });
            }

            return Task.CompletedTask;
        }

        private enum ServiceActions
        {
            Start,
            Stop
        }
    }
}