using DependencyInjectionApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.DependencyInjection
{
    public class ApplicationModule : BaseModule 
    {
        protected override void RegisterServiceModule(IServiceCollection serviceModule)
        {
            serviceModule.AddHostedService<AppStartupService>();
        }
    }
}