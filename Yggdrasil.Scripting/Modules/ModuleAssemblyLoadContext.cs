using System.Reflection;
using System.Runtime.Loader;

namespace Yggdrasil.Scripting.Modules
{
    internal class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly ModuleAssemblyResolver _moduleResolver;

        public ModuleAssemblyLoadContext(ModuleAssemblyResolver resolver, string moduleAssemblyPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(moduleAssemblyPath);
            _moduleResolver = resolver;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assembly = base.Load(assemblyName);
            if (assembly != null) { return assembly; }
            if (assemblyName.Name == null) { return null; }

            try { assembly = Default.LoadFromAssemblyName(assemblyName); }
            catch { }

            if (assembly != null) { return assembly; }

            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null) { return LoadFromAssemblyPath(assemblyPath); }


        }
    }
}
