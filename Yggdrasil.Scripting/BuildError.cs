using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yggdrasil.Scripting
{
    public class BuildError
    {
        public List<string> Data = new List<string>();
        public List<Diagnostic> Diagnostics = new List<Diagnostic>();
        public bool IsCritical;
        public string Message;
    }
}