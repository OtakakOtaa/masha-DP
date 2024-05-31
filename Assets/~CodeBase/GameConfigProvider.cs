using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Customers;
using _CodeBase.Customers._Data;
using _CodeBase.Garden.Data;
using _CodeBase.Potion.Data;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CodeBase
{
    [CreateAssetMenu(fileName = nameof(GameConfigProvider), menuName = nameof(GameConfigProvider))]
    public sealed partial class GameConfigProvider : ScriptableObject
    {
        [TabGroup("main")]
        [SerializeField] private PotionConfig[] _potionConfigs;
        
        [TabGroup("main")]
        [SerializeField] private PlantConfig[] _plantConfigs;
        
        [TabGroup("main")]
        [SerializeField] private EssenceConfig[] _essenceConfigs;
        
        [TabGroup("main")]
        [SerializeField] private CustomersConfiguration _customersConfiguration;
        
        [TabGroup("main")]
        [SerializeField] private string _mixerUniqId;
        
        [TabGroup("main")]
        [SerializeField] private MixEssenceVisualData[] _mixEssenceDictionary;
        
        [TabGroup("main")]
        [SerializeField] private CraftRowsData[] _craftRowsData;
        
        [TabGroup("PreGame Data")] 
        [SerializeField] private PersistentGameData _staticData;
        
        
        private Dictionary<string, IUniq> _browser;
        private Dictionary<string, Sprite> _craftRowsSpriteBrowser;
        private CustomerFarewellWord[] _goodCustomerFarewellWords;
        private CustomerFarewellWord[] _badCustomerFarewellWords;
        
        
        public int UniqOrderCount => _customersConfiguration.Orders.GroupBy(c => c.RequestedItemID).Count();
        public int UniqVisualCount => _customersConfiguration.CustomerVisuals.Count();
        public int UniqCustomerInfoCount => _customersConfiguration.CustomerInfos.Count();
        public PersistentGameData StaticData => PersistentGameData.Clone(_staticData);
        public IEnumerable<PotionConfig> Potions => _potionConfigs;
        public IEnumerable<string> AllEssencesIDs => _essenceConfigs.Select(e => e.ID);
        public IEnumerable<Order> Orders => _customersConfiguration.Orders;
        public string MixerUniqId => _mixerUniqId;
        public IEnumerable<CustomerFarewellWord> GoodCustomerFarewells
        {
            get
            {
                if (_goodCustomerFarewellWords.IsNullOrEmpty()) CreateFarewellsCollections();
                return _goodCustomerFarewellWords;
            }
        }

        public IEnumerable<CustomerFarewellWord> BadCustomerFarewells
        {
            get
            {
                if (_badCustomerFarewellWords.IsNullOrEmpty()) CreateFarewellsCollections();
                return _badCustomerFarewellWords;
            }
        }


        public TType GetByID<TType>(string id) where TType : IUniq
        {
            if (_browser is null) CreteBrowser();

            var value = _browser.GetValueOrDefault(id);
            if (value == default) return default;
            
            return (value is TType uniq ? uniq : default);
        }
        
        public UniqItemsType TryDefineTypeByID(string id)
        {
            if (_browser is null) CreteBrowser();

            var value = _browser.GetValueOrDefault(id);
            return value?.Type ?? UniqItemsType.None;
        }
        
        
        public TType GetRandomBasedOnWeight<TType>() where TType : PollEntity
        {
            TType[] pool = null;

            if (typeof(TType) == typeof(Order)) pool = _customersConfiguration.Orders.Cast<TType>().ToArray();
            if (typeof(TType) == typeof(CustomerInfo)) pool = _customersConfiguration.CustomerInfos.Cast<TType>().ToArray();
            if (typeof(TType) == typeof(CustomerVisual)) pool = _customersConfiguration.CustomerVisuals.Cast<TType>().ToArray();
            
            
            if (pool.Length == 0) throw new Exception($"CRITICAL ERROR : Orders list is empty");
            if (pool.Length == 1) return pool[0];
            
            var targetWeight = Random.Range(0f, 1f);
            var randomProvider = new System.Random();
            var shuffledPool = pool.OrderBy(_ => randomProvider.Next()).ToArray();

            var weightSum = 0f;
            foreach (var item in shuffledPool)
            {
                weightSum += item.Weight;
                if (weightSum >= targetWeight) return item;
            }

            return shuffledPool[0];
        }

        
        public CustomerFarewellWord GetRandomBasedOnWeightFarewell(bool goodnessFlag)
        {
            var pool = goodnessFlag ? GoodCustomerFarewells : BadCustomerFarewells;
            
            if (pool.Count() == 0) throw new Exception($"CRITICAL ERROR : Orders list is empty");
            if (pool.Count() == 1) return pool.ElementAt(0);
            
            var targetWeight = Random.Range(0f, 1f);
            var randomProvider = new System.Random();
            var shuffledPool = pool.OrderBy(_ => randomProvider.Next()).ToArray();

            var weightSum = 0f;
            foreach (var item in shuffledPool)
            {
                weightSum += item.Weight;
                if (weightSum >= targetWeight) return item;
            }

            return shuffledPool[0];
        }
        
        [CanBeNull]
        public MixEssenceVisualData GetEssenceMixVisualData(List<(string id, int amount)> essences)
        {
            var mixerComponentI = essences.FindIndex(e => e.id == _mixerUniqId);
            
            if (mixerComponentI != -1) essences.RemoveAt(mixerComponentI);
            
            
            foreach (var mixEssenceVisualData in _mixEssenceDictionary)
            {
                var isEqual = mixEssenceVisualData.CompoundData.All(cd =>
                {
                    var index = essences.FindIndex(e => e.id == cd.ID);
                    return index != -1 && essences[index].amount == cd.Amount;
                });

                if (isEqual) return mixEssenceVisualData;
            }

            return null;
        } 
        

        [CanBeNull]
        public Sprite GetCraftRowForPotionID(string id)
        {
            _craftRowsSpriteBrowser ??= _craftRowsData.ToDictionary(c => c.PotionID, c => c.Sprite);
            return _craftRowsSpriteBrowser.GetValueOrDefault(id);
        } 

        [CanBeNull]
        public Sprite GetSpriteByID(string id)
        {
            return _browser.GetValueOrDefault(id)?.Sprite;
        } 
        
        
        private void CreteBrowser()
        {
            _browser = _plantConfigs.Cast<IUniq>()
                .Concat(_essenceConfigs)
                .Concat(_potionConfigs)
                .Concat(_customersConfiguration.CustomerInfos)
                .Concat(_customersConfiguration.Orders)
                .Concat(_customersConfiguration.CustomerVisuals)
                .Where(c => !string.IsNullOrEmpty(c.ID))
                .ToDictionary(c => c.ID);
        }

        private void CreateFarewellsCollections()
        {
            _goodCustomerFarewellWords = _customersConfiguration.FarewellWords.Where(f => f.IsGoodMes).ToArray();
            _badCustomerFarewellWords = _customersConfiguration.FarewellWords.Where(f => f.IsGoodMes is false).ToArray();
        }
    }

    public enum UniqItemsType
    {
        None = -1,
        Potion = 0,
        Essence = 1,
        Plant = 2,
        CustomerVisual = 3, 
        CustomerInfo = 4,
        Order = 5,
        Booster = 6,
        CustomerFarewellWord = 7,
    }
    
    public interface IUniq
    {
        string ID { get; }
        string Name { get; }
        UniqItemsType Type { get; } 
        Sprite Sprite { get; }
    }

    [Serializable] public sealed class MixEssenceVisualData
    {
        [SerializeField] private PotionCompoundData[] _compoundData;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Color _color = UnityEngine.Color.white;
        
        
        public PotionCompoundData[] CompoundData => _compoundData;
        public Sprite Sprite => _sprite;
        public Color Color => _color;
    }
    

    [Serializable] public sealed class CraftRowsData
    {
        [ValueDropdown("@MashaEditorUtility.GetAllPotionsID()")]
        [SerializeField] private string _potionID;
        
        [SerializeField] private Sprite _sprite;
        
        public string PotionID => _potionID;
        public Sprite Sprite => _sprite;
    }
    
}