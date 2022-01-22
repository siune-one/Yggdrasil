using System;
using System.Xml.Serialization;
using Yggdrasil.Attributes;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Sequence : Node
    {
        public Sequence(Func<object, bool> conditional)
        {
            Conditional = conditional;
        }

        public Sequence() { }

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
                if (result == Result.Failure) { return result; }
            }

            return Result.Success;
        }

        private static bool DefaultConditional(object s)
        {
            return true;
        }
    }
}