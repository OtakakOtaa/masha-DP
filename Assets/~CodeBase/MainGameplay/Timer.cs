using System;
using UnityEngine;

namespace _CodeBase.MainGameplay
{
    public sealed class Timer
    {
        private float _startTime;
        private float _targetDTime;

        public bool IsTimeUp => _startTime + _targetDTime >= Time.time;

        public TimeSpan Value => TimeSpan.FromSeconds(Time.time - _startTime);
        
        public void Run(TimeSpan target)
        {
            _startTime = Time.time;
            _targetDTime = (float)target.TotalSeconds;
        }
        
    }
}