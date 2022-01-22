using System.Collections.Generic;
using System.Reflection;

namespace Yggdrasil.Scripting
{
    public class ScriptedFunction
    {
        public object Builder;
        public MethodInfo BuilderMethod;
        public string BuilderMethodName;
        public string FunctionMethodName;
        public string FunctionText;
        public string Guid;
        public PropertyInfo Property;
        public string PropertyName;
        public string ScriptText;

        public HashSet<string> References { get; set; } = new HashSet<string>();

        public void SetFunctionPropertyValue(object obj)
        {
            var function = BuilderMethod?.Invoke(Builder, null);
            Property.SetValue(obj, function);
        }
    }
}