using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text.Json;
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

        private static List<CompiledModule<T>> GetModuleMetadata(string modulesDir)
        {
            var output = new List<CompiledModule<T>>();
            if (!Directory.Exists(modulesDir)) {  return output; }

            foreach (var dir in Directory.GetDirectories(modulesDir))
            {
                var jsonFiles = Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories);
                var metadataFile = jsonFiles.FirstOrDefault(n => Path.GetFileName(n).ToLowerInvariant() == "info.json");
                if (metadataFile == null) {  continue; }

                var module = new CompiledModule<T>();
                module.Directory = dir;

                try
                {
                    module.Metadata = JsonSerializer.Deserialize<ModuleMetadata>(metadataFile);
                }
                catch (Exception e)
                {
                    module.Error = ModuleErrorType.MetadataDeserialization;
                    module.ErrorMessage = e.Message;
                }

                output.Add(module);
            }

            return output;
        }
    }
}
