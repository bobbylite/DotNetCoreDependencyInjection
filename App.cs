using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DependencyInjectionApp.DependencyInjection.Modules;

namespace DependencyInjectionApp
{
    public class App
    {
        public static void Main(string[] args) => HostBuilder(args).Build().Run();
        public static IHostBuilder HostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging=>
                {
                    logging.AddConsole();
                    logging.AddFile("./dependencyInjectionApp/logs/AppLog.log");
                })
                .ConfigureAppConfiguration((hostingContext, config)
                    => config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false))
                .ConfigureServices((services) => new ApplicationModule().BuildModule(services))
                .ConfigureServices((services) => new ServicesModule().BuildModule(services));
    }
}
