﻿using System;
using System.Runtime.CompilerServices;
using Yggdrasil.Utility;

namespace Yggdrasil.Coroutines
{
    [AsyncMethodBuilder(typeof(Coroutine<>))]
    public class Coroutine<T> : CoroutineBase, ICriticalNotifyCompletion, IContinuation
    {
        public static readonly ConcurrentPool<Coroutine<T>> Pool = new ConcurrentPool<Coroutine<T>>();

        private bool _isConstant;
        private T _result;
        private IStateMachineWrapper _stateMachine;

        public Coroutine<T> Task => this;

        public bool IsCompleted { get; private set; }

        public void MoveNext()
        {
            _stateMachine.MoveNext();
        }

        public void Discard()
        {
            if (_isConstant) { return; }

            _result = default;
            IsCompleted = false;

            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;

            Pool.Recycle(this);
        }

        public void OnCompleted(Action continuation) { }

        public void UnsafeOnCompleted(Action continuation) { }

        public static Coroutine<T> Create()
        {
            return Pool.Get();
        }

        public static Coroutine<T> CreateConst(T result)
        {
            var coroutine = Pool.Get();

            coroutine._result = result;
            coroutine.IsCompleted = true;
            coroutine._isConstant = true;

            return coroutine;
        }

        public Coroutine<T> GetAwaiter()
        {
            return this;
        }

        public T GetResult()
        {
            if (_isConstant) { return _result; }

            var result = _result;

            _result = default;
            IsCompleted = false;
            Pool.Recycle(this);

            return result;
        }

        public void SetResult(T result)
        {
            _result = result;
            IsCompleted = true;

            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;
        }

        public void SetException(Exception exception)
        {
            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;

            CoroutineManagerBase.CurrentInstance.SetException(exception);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) { }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            var wrapper = SmPool.Get<StateMachineWrapper<TStateMachine>>();
            wrapper.StateMachine = stateMachine;

            _stateMachine = wrapper;

            wrapper.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter _, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            CoroutineManagerBase.CurrentInstance.AddContinuation(this);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter _,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            CoroutineManagerBase.CurrentInstance.AddContinuation(this);
        }
    }

    [AsyncMethodBuilder(typeof(Coroutine))]
    public class Coroutine : CoroutineBase, ICriticalNotifyCompletion, IContinuation
    {
        public static readonly ConcurrentPool<Coroutine> Pool = new ConcurrentPool<Coroutine>();

        private bool _isConstant;
        private IStateMachineWrapper _stateMachine;

        public Coroutine Task => this;

        public bool IsCompleted { get; private set; }

        public void MoveNext()
        {
            _stateMachine.MoveNext();
        }

        public void Discard()
        {
            if (_isConstant) { return; }

            IsCompleted = false;

            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;

            Pool.Recycle(this);
        }

        public void OnCompleted(Action continuation) { }

        public void UnsafeOnCompleted(Action continuation) { }

        public static Coroutine Create()
        {
            return Pool.Get();
        }

        public static Coroutine CreateConst(bool isCompleted)
        {
            var coroutine = Pool.Get();

            coroutine.IsCompleted = isCompleted;
            coroutine._isConstant = true;

            return coroutine;
        }

        public Coroutine GetAwaiter()
        {
            return this;
        }

        public object GetResult()
        {
            if (_isConstant) { return null; }

            IsCompleted = false;
            Pool.Recycle(this);

            return null;
        }

        // This is never called for a lowermost instanced Coroutine, like the CoroutineManager's Yield.
        public void SetResult()
        {
            if (_isConstant) { return; }

            IsCompleted = true;

            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;
        }

        public void SetException(Exception exception)
        {
            if (_stateMachine != null) { SmPool.Recycle(_stateMachine); }

            _stateMachine = null;

            CoroutineManagerBase.CurrentInstance.SetException(exception);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine) { }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            var wrapper = SmPool.Get<StateMachineWrapper<TStateMachine>>();
            wrapper.StateMachine = stateMachine;

            _stateMachine = wrapper;

            wrapper.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            CoroutineManagerBase.CurrentInstance.AddContinuation(this);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
            ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            CoroutineManagerBase.CurrentInstance.AddContinuation(this);
        }
    }
}