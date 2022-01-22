using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Yggdrasil.Attributes;
using Yggdrasil.Coroutines;
using Yggdrasil.Enums;

namespace Yggdrasil.Behaviour
{
    public class Interrupt : Node
    {
        [XmlIgnore]
        private readonly List<CoroutineThread<Result>> _threads = new List<CoroutineThread<Result>>(10);

        public Interrupt(Func<object, bool> conditional)
        {
            Conditional = conditional;
        }

        public Interrupt() { }

        [XmlIgnore]
        [ScriptedFunction]
        public Func<object, bool> Conditional { get; set; } = DefaultConditional;

        public override void Terminate()
        {
            foreach (var thread in _threads) { thread.Reset(); }
        }

        public override void Initialize()
        {
            if (Children != null && Children.Count > 0 && _threads.Count <= 0)
            {
                foreach (var n in Children)
                {
                    var thread = new CoroutineThread<Result>(n.Execute, false, 1);
                    _threads.Add(thread);
                }
            }
        }

        protected override async Coroutine<Result> Tick()
        {
            if (!Conditional(State)) { return Result.Failure; }

            foreach (var thread in _threads)
            {
                thread.Reset();
                BehaviourTree.CurrentInstance.ProcessThreadAsDependency(thread);
            }

            while (Continue())
            {
                await Yield;

                if (!Conditional(State))
                {
                    foreach (var thread in _threads) { BehaviourTree.CurrentInstance.TerminateThread(thread); }

                    return Result.Failure;
                }
            }

            var result = Result.Failure;

            foreach (var thread in _threads)
            {
                if (thread.Result == Result.Success) { result = Result.Success; }

                thread.Reset();
            }

            return result;
        }

        private bool Continue()
        {
            var processing = false;
            foreach (var thread in _threads) { processing = processing || !thread.IsComplete; }

            return processing;
        }

        private static bool DefaultConditional(object s)
        {
            return true;
        }
    }
}