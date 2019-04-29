using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Interfaces
{
    public interface IAssemblyLoader
    {
        Assembly[] Load(Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func);
    }

    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        private static readonly string ThisAssemblyName = typeof(DefaultAssemblyLoader).GetTypeInfo().Assembly.GetShortName();

        public Assembly[] Load(Func<IEnumerable<Assembly>, IEnumerable<Assembly>> func)
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries;

            var assemblies = new List<Assembly>();
            var entryAssembly = Assembly.GetEntryAssembly();
            var entryAssemblyName = entryAssembly.GetShortName();
            var directDependencies = (from library in dependencies
                                      where library.Name.Equals(entryAssemblyName, StringComparison.OrdinalIgnoreCase)
                                            || library.Name.Equals(ThisAssemblyName, StringComparison.OrdinalIgnoreCase)
                                            || library.Dependencies.Any(d =>
                                                d.Name.Equals(ThisAssemblyName, StringComparison.OrdinalIgnoreCase))
                                      select library).ToList();

            var entryLibrary = directDependencies.FirstOrDefault(d => d.Name.Equals(entryAssemblyName, StringComparison.OrdinalIgnoreCase));

            // 
            if (entryLibrary != null)
            {
                var entryDependencies = entryLibrary.Dependencies.Where(dependency => !directDependencies.Any(d => d.Name.Equals(dependency.Name, StringComparison.OrdinalIgnoreCase)));
                foreach (var entryDependency in entryDependencies)
                {
                    try
                    {
                        var assembly = Assembly.Load(new AssemblyName(entryDependency.Name));
                        assemblies.Add(assembly);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }

            //
            foreach (var directDependency in directDependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(directDependency.Name));
                    assemblies.Add(assembly);
                }
                catch
                {
                    // ignore
                }
            }

            if (func == null) return assemblies.ToArray();
            var more = func(assemblies);
            assemblies.AddRange(more);

            return assemblies.ToArray();
        }
    }

    public static class AssemblyLoaderExtensions
    {
        public static string ExtractAssemblyShortName(this IAssemblyLoader assemblyLoader, string fullName)
        {
            return ExtractAssemblyShortName(fullName);
        }

        public static string ExtractAssemblyShortName(string fullName)
        {
            fullName = fullName.ToLowerInvariant();
            var index = fullName.IndexOf(',');
            return index < 0 ? fullName : fullName.Substring(0, index);
        }

        public static string GetShortName(this Assembly assembly)
        {
            return ExtractAssemblyShortName(assembly.FullName);
        }

        public static string GetAssemblyNameFromFileName(string fileName)
        {
            fileName = fileName.ToLowerInvariant();
            return fileName.EndsWith(".dll")
                ? fileName : fileName.Substring(0, fileName.Length - 4);
        }
    }
}
