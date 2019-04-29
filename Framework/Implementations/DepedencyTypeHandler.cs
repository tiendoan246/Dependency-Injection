using Framework.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Framework.Implementations
{
    public class DepedencyTypeHandler : ITypeHandler
    {
        private readonly Type _depedencyType = typeof(IDependency);
        private readonly Type _scopDepedencyType = typeof(IScopeDepedency);
        private readonly Type _singletonDepedencyType = typeof(ISingletonDepedency);

        private IServiceCollection _builder;

        public DepedencyTypeHandler(IServiceCollection builder)
        {
            _builder = builder;
        }

        public void Process(Type type)
        {
            if (!_depedencyType.IsAssignableFrom(type))
            {
                return;
            }

            foreach (var interfaceType in type.GetInterfaces().Where(i => _depedencyType.IsAssignableFrom(i)))
            {
                if (_singletonDepedencyType.IsAssignableFrom(interfaceType))
                {
                    _builder.AddSingleton(interfaceType, type);
                }
                else if (_scopDepedencyType.IsAssignableFrom(interfaceType))
                {
                    _builder.AddScoped(interfaceType, type);
                }
                else
                {
                    _builder.AddTransient(interfaceType, type);
                }
            }
        }
    }
}
