using DependencyInjectionApp.Services;
using DependencyInjectionApp.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.DependencyInjection.Modules
{
    public class ServicesModule : BaseModule 
    {
        protected override void RegisterServiceModule(IServiceCollection serviceModule)
        {
            serviceModule.AddSingleton<IAutoStart, WebEndpointService>();
        }
    }
}