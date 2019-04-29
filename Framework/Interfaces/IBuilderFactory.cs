using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Interfaces
{
    public interface IBuilderFactory
    {
        void Create(Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func);
    }

    public class BuilderFactory : IBuilderFactory
    {
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly IAssemblyHandler _assemblyHandler;
        public BuilderFactory(IAssemblyLoader assemblyLoader, IAssemblyHandler assemblyHandler)
        {
            _assemblyLoader = assemblyLoader;
            _assemblyHandler = assemblyHandler;
        }

        public void Create(Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func)
        {
            var thisAssembly = GetType().GetTypeInfo().Assembly;

            var thisAssemblyFullName = thisAssembly.FullName;

            var assemblies = _assemblyLoader.Load(func)
                .Where(a => a.GetReferencedAssemblies().Any(asb => string.CompareOrdinal(asb.FullName, thisAssemblyFullName) == 0));

            _assemblyHandler.Process(thisAssembly);
            foreach (var assembly in assemblies)
            {
                _assemblyHandler.Process(assembly);
            }
        }
    }
}
