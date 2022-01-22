using System;
using System.Xml.Serialization;
using Yggdrasil.Attributes;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Condition : Node
    {
        public Condition(Func<object, bool> conditional)
        {
            Conditional = conditional;
        }

        public Condition() { }

        [XmlIgnore]
        [ScriptedFunction]
        public Func<object, bool> Conditional { get; set; } = DefaultConditional;

        protected override Coroutine<Result> Tick()
        {
            return Conditional(State) ? Success : Failure;
        }

        private static bool DefaultConditional(object s)
        {
            return true;
        }
    }
}