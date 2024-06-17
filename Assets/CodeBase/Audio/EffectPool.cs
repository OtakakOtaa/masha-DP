using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeBase.Audio
{
    [Serializable] public sealed class EffectPool
    {
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string[] _effects;
        
        public string[] Effects => _effects;

        public string GetRandomEffect()
        {
            return _effects[UnityEngine.Random.Range(0, _effects.Length)];
        }
    }
}