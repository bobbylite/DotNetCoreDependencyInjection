using Microsoft.Extensions.DependencyInjection;
using DependencyInjectionApp.Common;

namespace DependencyInjectionApp.DependencyInjection
{
    public abstract class BaseModule : IBaseModule 
    {
        public void BuildModule(IServiceCollection service)
        {
            try 
            {
                RegisterServiceModule(service);
            } catch
            {
            }
        }

        protected abstract void RegisterServiceModule(IServiceCollection serviceModule);
    }
}