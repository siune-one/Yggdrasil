using System.Collections.Generic;

namespace Yggdrasil.Scripting
{
    public class YggParserConfig
    {
        public bool ReplaceObjectStateWithDynamic { get; set; }

        public HashSet<string> ReferenceAssemblyPaths { get; set; } = new HashSet<string>();

        public HashSet<string> NodeTypeAssemblies { get; set; } = new HashSet<string>();

        public HashSet<string> ScriptNamespaces { get; set; } = new HashSet<string>();
    }
}