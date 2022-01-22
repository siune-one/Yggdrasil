using System.Xml.Serialization;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Inverter : Node
    {
        [XmlIgnore]
        public Node Child
        {
            get
            {
                if (Children == null || Children.Count <= 0) { return null; }

                return Children[0];
            }
        }

        protected override async Coroutine<Result> Tick()
        {
            if (Child == null) { return Result.Failure; }

            var result = await Child.Execute();
            if (result == Result.Unknown) { return result; }

            return result == Result.Success ? Result.Failure : Result.Success;
        }
    }
}