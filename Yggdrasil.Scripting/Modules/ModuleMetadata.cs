using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yggdrasil.Scripting.Modules
{
    [DataContract]
    public class ModuleMetadata
    {
        [DataMember(Order = 1)]
        public string Name;

        [DataMember(Order = 2)]
        public string MainAssemblyName;

        [DataMember(Order = 3)]
        public string AssemblyDirectory;

        [DataMember(Order = 4)]
        public string ScriptDirectory;

        [DataMember(Order = 5)]
        public string Version;

        [DataMember(Order = 6)]
        public string VersionCheckUrl;

        [DataMember(Order = 7)]
        public string DownloadUrl;

        [DataMember(Order = 8)]
        public List<string> Dependencies = new List<string>();
    }
}
