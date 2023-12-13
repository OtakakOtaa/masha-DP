using System;
using System.Collections.Generic;

namespace _CodeBase.Infrastructure
{
    public sealed class SubscribesToken : IDisposable
    {
        private readonly List<Action> _source = new();

        public void Add(Action action)
        {
            _source.Add(action);
        }
        
        public void Dispose()
        {
            foreach (var item in _source) item?.Invoke();
            _source.Clear();
        }
    }
}