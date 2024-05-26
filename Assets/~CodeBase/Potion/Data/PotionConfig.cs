using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _CodeBase.Potion.Data
{
    [CreateAssetMenu(fileName = nameof(PotionConfig), menuName = nameof(PotionConfig), order = 0)]
    public sealed class PotionConfig : ScriptableObject, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private PotionCompoundData[] _compound;
        [SerializeField] private int _tier = 1;
        [SerializeField] private Color _color = new Color(0, 0, 0, 1);

        private Dictionary<string, PotionCompoundData> _browser;
        
        
#if UNITY_EDITOR

        [Button("Generate ID")]
        public void GenerateID()
        {
            _id = name.Trim();
            EditorUtility.SetDirty(this);
        }
#endif
        
        public int this[string compoundId]
        {
            get
            {
                _browser ??= _compound.ToDictionary(c => c.ID, c => c);
                return _browser.TryGetValue(compoundId, out var value) ? value.Amount : 0;
            }
        }
        
        
        public string ID => _id.Trim();
        public IEnumerable<PotionCompoundData> Compound => _compound;
        public string Name => _name;
        public UniqItemsType Type => UniqItemsType.Potion;
        public int Tier => _tier;
        public Color Color => _color;
    }
    

    
    [Serializable] public sealed class PotionCompoundData
    {
        [ValueDropdown("@MashaEditorUtility.GetAllEssencesAndPlantsID()")]
        [HorizontalGroup]
        [SerializeField] private string _id;
        
        [HorizontalGroup(30)]
        [HideLabel]
        [SerializeField] private int _amount = 1;
        
        [SerializeField] private UniqItemsType _type;
        
        
        
        public string ID => _id;
        public int Amount => _amount;
        public UniqItemsType Type => _type;


#if UNITY_EDITOR
        public void SetData(UniqItemsType uniqItemsType)
        {
            _type = uniqItemsType;
        }
#endif
    }
}