using Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace WebApplication
{
    public class DiConfig
    {
        public static void ConfigServices(IServiceCollection services, IConfiguration config)
        {
            Starter.RegisterServices(services, config,
                initBuilder =>
                {

                },
                loadAssembly =>
                {
                    return Enumerable.Empty<Assembly>();
                });
        }
    }
}
