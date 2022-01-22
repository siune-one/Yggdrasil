using System;
using System.Xml.Serialization;
using Yggdrasil.Attributes;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Selector : Node
    {
        public Selector(Func<object, bool> conditional)
        {
            Conditional = conditional;
        }

        public Selector() { }

        [XmlIgnore]
        [ScriptedFunction]
        public Func<object, bool> Conditional { get; set; } = DefaultConditional;

        protected override async Coroutine<Result> Tick()
        {
            if (Children == null || Children.Count <= 0) { return Result.Failure; }
            if (!Conditional(State)) { return Result.Failure; }

            foreach (var child in Children)
            {
                var result = await child.Execute();
                if (result == Result.Success) { return result; }
            }

            return Result.Failure;
        }

        private static bool DefaultConditional(object s)
        {
            return true;
        }
    }
}