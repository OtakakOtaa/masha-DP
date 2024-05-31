using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Customers
{
    [CreateAssetMenu(fileName = nameof(ShopConfigurationProvider))]
    public sealed class ShopConfigurationProvider : ScriptableObject
    {
        [SerializeField] private ShopItemData[] _shopItemData;

        private Dictionary<string, ShopItemData> _browser;
        
        public ShopItemData[] ShopItemData => _shopItemData;

        
        [CanBeNull]
        public ShopItemData GetDataByID(string id)
        {
            _browser ??= _shopItemData.ToDictionary(s => s.ID);


            return _browser.GetValueOrDefault(id);
        }
    }
    
    
    [Serializable] public sealed class ShopItemData
    {
        [ValueDropdown("@MashaEditorUtility.GetAllEssencesAndPlantsID()")]
        [SerializeField] private string _id;
        
        [SerializeField] private int _cost;
        [TextArea]
        [SerializeField] private string _name;
        [SerializeField] private Vector2 _rect;
        
        
        public int Cost => _cost;
        public string ID => _id;
        public string Name => _name;
        public Vector2 Rect => _rect;
    } 

}