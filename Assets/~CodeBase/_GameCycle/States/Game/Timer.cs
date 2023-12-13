using System;
using UnityEngine;

namespace _CodeBase._GameCycle.States.Game
{
    public sealed class Timer
    {
        private float _startTime;
        private float _targetDTime;

        public bool IsTimeUp => _startTime + _targetDTime >= Time.time; 
        
        public void Run(TimeSpan target)
        {
            _startTime = Time.time;
            _targetDTime = (float)target.TotalSeconds;
        }
        
    }
}