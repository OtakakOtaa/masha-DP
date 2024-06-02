using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _CodeBase.DATA
{
    [CreateAssetMenu(fileName = nameof(BoosterConfigs))]
    public sealed class BoosterConfigs : ScriptableObject, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private string _name;
        [SerializeField] private float _value;


        public float Value => _value;
        public string ID => _id;
        public string Name => _name;
        public UniqItemsType Type => UniqItemsType.Booster;
        public Sprite Sprite => _sprite;
        
#if UNITY_EDITOR

        [Button("Generate ID")]
        public void GenerateID()
        {
            _id = name.Trim();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}