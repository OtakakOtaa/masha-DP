using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace _CodeBase.MainGameplay
{
    [Serializable] public sealed class GameplayData
    {
        public const int InfinityLandingValue = -1; 
        
        [SerializeField] private int _coins;
        [SerializeField] private int _servedCustomersAmount;
        
        private readonly Dictionary<string, int> _availablePlants = new();
        private readonly Dictionary<string, int> _availablePlantsPoolForLandings = new();
        private readonly HashSet<string> _availableEssences = new();
        private readonly HashSet<string> _uniqItems = new();
        
        public readonly ReactiveCommand<int> CoinsChanged = new();
        public readonly ReactiveCommand<int> ServedCustomersAmountChanged = new();
        public readonly ReactiveCommand<(string type, int amount)> PlantWasAdded = new();
        public readonly ReactiveCommand<(string type, int amount)> PlantWasRemoved = new();


        public int Coins => _coins;
        public int ServedCustomersAmount => _servedCustomersAmount;
        public IEnumerable<string> AvailablePlantsLanding => _availablePlantsPoolForLandings.Keys; 
        public IEnumerable<string> AvailablePlantsStorage => _availablePlants.Keys;
        public IEnumerable<string> AllEssences => _availableEssences;
        public IEnumerable<string> UniqItems => _uniqItems;

        
        public int GetPlantsCountForLanding(string key)
        {
            return _availablePlantsPoolForLandings.GetValueOrDefault(key);
        }

        public void AddPlantsToLandingPool(string key, int amount)
        {
            _availablePlantsPoolForLandings[key] = amount;
        }

        public bool TryRemovePlantsForLanding(string key, int amount)
        {
            var currentAmount = _availablePlantsPoolForLandings.GetValueOrDefault(key);
            if (currentAmount is InfinityLandingValue) return true;
            
            if (currentAmount is 0 || currentAmount - amount < 0) return false;

            _availablePlantsPoolForLandings[key] = currentAmount - amount;
            return true;
        }
        
        public void ChangeCoinBalance(int finalValue)
        {
            if (finalValue < 0) throw new Exception($"{typeof(GameplayData).FullName} : {nameof(ChangeCoinBalance)} try set as 0 or less");

            _coins = finalValue;
            CoinsChanged.Execute(finalValue);
        }

        public void IncreaseServedCustomers()
        {
            ServedCustomersAmountChanged.Execute(++_servedCustomersAmount);
        }
        
        public void AddPlant(string plantType)
        {
            _availablePlants[plantType] = _availablePlants.TryGetValue(plantType, out var plant) ? plant + 1 : 1;
            PlantWasAdded.Execute((plantType, amount: 1));
        }

        public bool TryRemovePlant(string plantType)
        {
            if (!_availablePlants.ContainsKey(plantType) || _availablePlants[plantType] == 0) return false;

            _availablePlants[plantType] -= 1;

            PlantWasRemoved.Execute((plantType, amount: 1));
            return true;
        }

        public bool CheckPlantContains(string plantType)
        {
            return _availablePlants.ContainsKey(plantType) && _availablePlants[plantType] > 0;
        }

        public void AddEssence(string id)
        {
            _availableEssences.Add(id);
        }

        public void AddUniqItem(string id)
        {
            _uniqItems.Add(id);
        }
    }
}