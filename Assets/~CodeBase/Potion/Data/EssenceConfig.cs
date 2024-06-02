using System;
using _CodeBase.DATA;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _CodeBase.Potion.Data
{
    [CreateAssetMenu(menuName = "Create EssenceConfig", fileName = "EssenceConfig", order = 0)]
    [Serializable] public sealed class EssenceConfig : ScriptableObject, IUniq 
    {
        [SerializeField] private string _id;
        [SerializeField] private Color _color;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        
        [SerializeField] private float _regenDuration = 5f; 
        [SerializeField] private int _sipCount = 4;
        
#if UNITY_EDITOR

        [Button("Generate ID")]
        public void GenerateID()
        {
            _id = name.Trim();
            EditorUtility.SetDirty(this);
        }
#endif
        
        public string ID => _id.Trim();
        public string Name => _name.Trim();
        public UniqItemsType Type => UniqItemsType.Essence;
        public Color Color => _color;
        public Sprite Sprite => _sprite;
        
        public float RegenDuration => _regenDuration;
        public int SipCount => _sipCount;
        
    }
}