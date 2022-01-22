using System.Collections.Generic;

namespace Yggdrasil.Scripting
{
    public class YggCompilation
    {
        public List<BuildError> Errors = new List<BuildError>();

        public Dictionary<string, List<ScriptedFunction>> GuidFunctionMap =
            new Dictionary<string, List<ScriptedFunction>>();
    }
}