using System;
using System.Collections.Generic;

namespace _CodeBase.Infrastructure
{
    public sealed class Signal : IDisposable, ISignal
    {
        private Action _trigger;
        private readonly List<Action> _subscribers = new();
        
        public void Execute()
            => _trigger?.Invoke();

        public void Subscribe(Action action, SubscribesToken disposable)
        {
            _subscribers.Add(action);
            _trigger += action;
            disposable.Add(() => _trigger -= action);
        }

        public void Unsubscribe(Action action)
        {
            _subscribers.Remove(action);
            _trigger -= action;
        }

        public void Dispose() 
        {
            foreach (var subscriber in _subscribers) 
                _trigger -= subscriber;
        }
    }

    public interface ISignal
    {
        void Subscribe(Action action, SubscribesToken disposable);
        void Unsubscribe(Action action);
    }
}