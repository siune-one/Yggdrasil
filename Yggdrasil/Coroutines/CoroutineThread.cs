using System;
using System.Collections.Generic;

namespace Yggdrasil.Coroutines
{
    public class CoroutineThread<T>
    {
        private readonly List<IContinuation> _continuations = new List<IContinuation>(100);
        private readonly Stack<IContinuation> _continuationsBuffer = new Stack<IContinuation>(100);

        private Coroutine<T> _rootCoroutine;

        public CoroutineThread(Func<Coroutine<T>> root, bool neverCompletes, ulong ticksToComplete)
        {
            Root = root;
            TicksToComplete = ticksToComplete;
            NeverCompletes = neverCompletes;
        }

        // Outbound dependencies are those which need this thread to complete before they can complete.
        internal HashSet<CoroutineThread<T>> OutputDependencies { get; } = new HashSet<CoroutineThread<T>>();

        // Inbound dependencies are those which this thread depends on to complete before itself can complete.
        internal HashSet<CoroutineThread<T>> InputDependencies { get; } = new HashSet<CoroutineThread<T>>();

        public Func<Coroutine<T>> Root { get; }

        public ulong TicksToComplete { get; }

        public bool NeverCompletes { get; }

        internal bool DependenciesFinished { get; set; }

        public bool IsComplete { get; private set; }

        public T Result { get; private set; }

        public bool IsRunning { get; private set; }

        public ulong TickCount { get; private set; }

        public void Tick()
        {
            if (Root == null) { return; }

            IsRunning = true;

            if (_continuations.Count > 0)
            {
                do
                {
                    var next = _continuations[_continuations.Count - 1];
                    _continuations.RemoveAt(_continuations.Count - 1);

                    next.MoveNext();
                }
                while (_continuationsBuffer.Count == 0 && _continuations.Count > 0);
            }
            else
            {
                Result = default;
                _rootCoroutine = Root();
            }

            ConsumeBuffers();

            IsRunning = _continuations.Count > 0;
        }

        public void Reset()
        {
            IsRunning = false;
            TickCount = 0;
            IsComplete = false;
            Result = default;

            foreach (var cont in _continuations) { cont.Discard(); }

            _continuations.Clear();

            foreach (var cont in _continuationsBuffer) { cont.Discard(); }

            _continuationsBuffer.Clear();

            InputDependencies.Clear();
            OutputDependencies.Clear();
            DependenciesFinished = false;
        }

        internal void AddContinuation(IContinuation continuation)
        {
            _continuationsBuffer.Push(continuation);
        }

        private void ConsumeBuffers()
        {
            foreach (var continuation in _continuationsBuffer) { _continuations.Add(continuation); }

            _continuationsBuffer.Clear();

            // The tick finished for the whole subtree.
            if (_continuations.Count == 0)
            {
                // Forces recycling, otherwise GetResult() is never called for the root coroutine.
                Result = _rootCoroutine.GetResult();
                TickCount += 1;
                IsComplete = !NeverCompletes && TickCount >= TicksToComplete;
            }
        }
    }
}