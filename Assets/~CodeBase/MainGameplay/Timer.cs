using System;
using UnityEngine;

namespace _CodeBase.MainGameplay
{
    public sealed class Timer
    {
        private float _startTime;
        private float _targetDTime;

        public bool IsTimeUp => Time.time >= _startTime + _targetDTime;
        public TimeSpan Value => TimeSpan.FromSeconds(Time.time - _startTime);
        public float TimeRatio => Mathf.Clamp01((Time.time - _startTime) / _targetDTime);
        
        
        public void RunWithDuration(TimeSpan duration)
        {
            _startTime = Time.time;
            _targetDTime = (float)duration.TotalSeconds;
        }

        public void RunWithDuration(float duration)
        {
            _startTime = Time.time;
            _targetDTime = duration;
        }
        
        
        public void RunWithEndPoint(TimeSpan targetPoint)
        {
            _startTime = Time.time;
            _targetDTime = (float)(targetPoint.TotalSeconds - Time.time);
        }
    }
}