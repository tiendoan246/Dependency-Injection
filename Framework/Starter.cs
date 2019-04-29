using Framework.Implementations;
using Framework.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
    public static class Starter
    {
        private static IServiceProvider _serviceProvider;

        public static void InitServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void RegisterServices(IServiceCollection builder, IConfiguration config, Action<IServiceCollection> initBuilderAction, Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func = null)
        {
            InitBuilder(builder, config, initBuilderAction, func);
        }

        private static void InitBuilder(IServiceCollection builder, IConfiguration config, Action<IServiceCollection> initBuilderAction, Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func)
        {
            var startupBuilder = new ServiceCollection();

            startupBuilder.AddSingleton(config);
            initBuilderAction(startupBuilder);

            startupBuilder.AddSingleton(builder);
            startupBuilder.AddSingleton<IAssemblyHandler, AssemblyHandler>();
            startupBuilder.AddSingleton<ITypeHandler, DepedencyTypeHandler>();
            startupBuilder.AddSingleton<IBuilderFactory, BuilderFactory>();
            startupBuilder.AddSingleton<IAssemblyLoader, DefaultAssemblyLoader>();

            var startupContainer = startupBuilder.BuildServiceProvider();
            var factory = startupContainer.GetService<IBuilderFactory>();
            factory.Create(func);
        }
    }
}
