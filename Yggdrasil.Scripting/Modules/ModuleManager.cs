using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Yggdrasil.Scripting.Modules
{
    public class ModuleManager<T> where T : IModule
    {
        private AssemblyLoadContext _loadContext;

        public List<CompiledModule<T>> Modules = new List<CompiledModule<T>>();

        public async Task Load(string modulesDir, YggParserConfig config)
        {
            Unload();

            var metadata = GetModuleMetadata(modulesDir);
            var loadOrder = new List<string>();



            var compiler = new YggCompiler();
            _loadContext = new AssemblyLoadContext(null, true);


        }

        public void Unload()
        {
            Modules.Clear();
            _loadContext?.Unload();
            _loadContext = null;
        }

        private List<(string Dir, ModuleInfo Info)> GetModuleMetadata(string modulesDir)
        {

        }
    }
}
