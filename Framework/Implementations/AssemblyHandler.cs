using Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Implementations
{
    public class AssemblyHandler : IAssemblyHandler
    {
        public readonly IEnumerable<ITypeHandler> _typeHandlers;

        public AssemblyHandler(IEnumerable<ITypeHandler> typeHandlers)
        {
            _typeHandlers = typeHandlers;
        }

        public void Process(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes().Where(TypeExpression))
            {
                foreach (var handler in _typeHandlers)
                {
                    handler.Process(type);
                }
            }
        }

        private bool TypeExpression(Type type)
        {
            var info = type.GetTypeInfo();

            return info.IsClass
                && info.IsPublic
                && ! info.IsAbstract
                && !info.IsGenericType;
        }
    }
}
