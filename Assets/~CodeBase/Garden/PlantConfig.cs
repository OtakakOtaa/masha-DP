using System;
using System.Collections.Generic;
using UnityEngine;

namespace _CodeBase.Garden
{
    [CreateAssetMenu(fileName = nameof(PlantConfig), menuName = nameof(PlantConfig))]
    public sealed class PlantConfig : ScriptableObject
    {
        [SerializeField] private PlantType _plantType;
        [SerializeField] private float _growTime;
        [SerializeField] private Sprite _sprite;
        
        [SerializeField] private List<PlatPhase>_platPhases;

        public PlantType PlantType => _plantType;
        public double GrowTime => _growTime;
        public List<PlatPhase> PlatPhases => _platPhases;

        public Sprite Sprite => _sprite;
    }

    [Serializable] public sealed class PlatPhase
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private float _progress;

        public PlatPhase(Sprite sprite, float progress)
        {
            this._sprite = sprite;
            this._progress = progress;
        }

        public Sprite Sprite => _sprite;
        public float Progress => _progress;
    }
}