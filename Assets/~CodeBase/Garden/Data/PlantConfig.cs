using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _CodeBase.Garden.Data
{
    [CreateAssetMenu(fileName = nameof(PlantConfig), menuName = nameof(PlantConfig))]
    public sealed class PlantConfig : ScriptableObject, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private float _growTime;
        [SerializeField] private Sprite _sprite;
        
        [SerializeField] private List<PlatPhase> _platPhases;

#if UNITY_EDITOR

        [Button("Generate ID")]
        public void GenerateID()
        {
            _id = name.Trim();
            EditorUtility.SetDirty(this);
        }
#endif
        
        public string ID => _id.Trim();
        public string Name => _name;
        public UniqItemsType Type => UniqItemsType.Plant;
        public double GrowTime => _growTime;
        public List<PlatPhase> PlatPhases => _platPhases;
        public Sprite Sprite => _sprite;
    }

    [Serializable] public sealed class PlatPhase
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private float _progress;
        
        
        public Sprite Sprite => _sprite;
        public float Progress => _progress;
    }
}