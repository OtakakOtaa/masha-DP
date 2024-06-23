using System;
using UnityEngine;

namespace _CodeBase.Garden.Data
{
    [Serializable] public sealed class GardenBedData
    {
        [SerializeField] private double _plantingTimePoint;
        [SerializeField] public bool hasPlant;

        private TimeSpan _plantingTimePointTs;
        private bool _isPlantingTimePointTsMapped = false;

        public TimeSpan PlantingTimePoint
        {
            get
            {
                if (_isPlantingTimePointTsMapped) return _plantingTimePointTs;

                _plantingTimePointTs = TimeSpan.FromSeconds(_plantingTimePoint);
                _isPlantingTimePointTsMapped = true;
                return _plantingTimePointTs;
            }
            set
            {
                _plantingTimePoint = value.TotalSeconds;
                _isPlantingTimePointTsMapped = false;
            }
        }
    }
}