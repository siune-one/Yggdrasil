using System;
using System.Xml.Serialization;
using Yggdrasil.Attributes;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Filter : Node
    {
        public Filter(Func<object, bool> conditional)
        {
            Conditional = conditional;
        }

        public Filter() { }

        [XmlIgnore]
        public Node Child
        {
            get
            {
                if (Children == null || Children.Count <= 0) { return null; }

                return Children[0];
            }
        }

        [XmlIgnore]
        [ScriptedFunction]
        public Func<object, bool> Conditional { get; set; } = DefaultConditional;

        protected override async Coroutine<Result> Tick()
        {
            if (Child == null) { return Result.Failure; }

            if (!Conditional(State)) { return Result.Failure; }

            return await Child.Execute();
        }

        private static bool DefaultConditional(object s)
        {
            return true;
        }
    }
}