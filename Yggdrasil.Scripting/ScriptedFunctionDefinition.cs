using System.Reflection;

namespace Yggdrasil.Scripting
{
    public class ScriptedFunctionDefinition
    {
        public PropertyInfo FunctionProperty;
        public string FunctionText;
        public string Guid;
        public bool ReplaceObjectWithDynamic;
    }
}