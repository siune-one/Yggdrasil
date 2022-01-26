using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Yggdrasil.Scripting.Modules
{
    internal class ModuleAssemblyResolver
    {
        private readonly Dictionary<string, Assembly> _nameToAssembly = new();
        private readonly Dictionary<string, string> _nameToPath = new();

        public Assembly GetOrLoad(AssemblyLoadContext ctx, string assemblySimpleName)
        {
            if (_nameToAssembly.TryGetValue(assemblySimpleName, out Assembly assembly))
            {
                return assembly;
            }

            if (_nameToPath.TryGetValue(assemblySimpleName, out var path))
            {
                assembly = ctx.LoadFromAssemblyPath(path);
                _nameToAssembly[assemblySimpleName] = assembly;
                return assembly;
            }

            return null;
        }

        public void Add(Assembly assembly)
        {
            _nameToAssembly[assembly.GetName().Name] = assembly;
        }

        public void Add(string assemblySimpleName, string assemblyPath)
        {
            _nameToPath[assemblySimpleName] = assemblyPath;
        }

        public void Clear()
        {
            _nameToAssembly.Clear();
            _nameToPath.Clear();
        }
    }
}
