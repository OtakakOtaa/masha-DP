using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Infrastructure
{
    [Serializable] public class STimeSpan
    {
        [HorizontalGroup]
        [ProgressBar(0, 59), LabelWidth(30)]
        [SerializeField] private int _min;

        [HorizontalGroup]
        [ProgressBar(0, 59), LabelWidth(30)]
        [SerializeField] private int _sec;

        [HorizontalGroup]
        [ProgressBar(0, 999), LabelWidth(50)]
        [SerializeField] private int _milSec;


        public STimeSpan(TimeSpan timeSpan)
        {
            _min = timeSpan.Minutes;
            _sec = timeSpan.Seconds;
            _milSec = timeSpan.Milliseconds;
        }

        public TimeSpan Value => new(days: 0, hours: 0, minutes: _min, seconds: _sec, milliseconds: _milSec);
    }
}