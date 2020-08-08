using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionApp.Common
{
    public interface IBaseModule
    {
        void BuildModule(IServiceCollection service);
    }
}