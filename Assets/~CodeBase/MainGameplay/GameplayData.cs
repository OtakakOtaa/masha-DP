﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;


namespace _CodeBase.MainGameplay
{
    [Serializable] public sealed class GameplayData
    {
        public const int InfinityLandingValue = -1; 
        
        [SerializeField] private int _coins;
        [SerializeField] private int _servedCustomersAmount;
        [SerializeField] private string _craftedPotion;
        
        private readonly Dictionary<string, int> _availablePlants = new();
        private readonly Dictionary<string, int> _availablePlantsPoolForLandings = new();
        private readonly HashSet<string> _availableEssences = new();
        private readonly HashSet<string> _uniqItems = new();

        public readonly ReactiveCommand<string> DataAddedEvent = new();
        public readonly ReactiveCommand<int> CreatedPotionEvent = new();
        public readonly ReactiveCommand<int> CoinsBalanceChangedEvent = new();
        public readonly ReactiveCommand<int> ServedCustomersAmountChangedEvent = new();
        public readonly ReactiveCommand<(string type, int amount)> PlantWasAddedEvent = new();
        public readonly ReactiveCommand<(string type, int amount)> PlantWasRemovedEvent = new();
        

        public int Coins => _coins;
        public int ServedCustomersAmount => _servedCustomersAmount;
        public IEnumerable<string> Seeds => _availablePlantsPoolForLandings.Keys; 
        public IEnumerable<string> HarvestPlants => _availablePlants.Keys;
        public IEnumerable<string> AccessibleEssences => _availableEssences;
        public IEnumerable<string> UniqItems => _uniqItems;
        [CanBeNull] public string CraftedPotion => _craftedPotion;
        
        
        public int GetPlantsCountForLanding(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new Exception(nameof(GetPlantsCountForLanding));
            
            return _availablePlantsPoolForLandings.GetValueOrDefault(key);
        }

        public void AddPlantsToLandingPool(string key, int amount)
        {
            if (string.IsNullOrEmpty(key) || (amount <= 0 && amount != InfinityLandingValue)) throw new Exception(nameof(AddPlantsToLandingPool));
            
            _availablePlantsPoolForLandings[key] = amount;

            DataAddedEvent?.Execute(key);
        }

        public bool TryRemovePlantsForLanding(string key, int amount)
        {
            if (string.IsNullOrEmpty(key)) throw new Exception(nameof(TryRemovePlantsForLanding));
            
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
            CoinsBalanceChangedEvent.Execute(finalValue);
        }

        public bool TryWithdrawCoins(int amount)
        {
            if (amount <= 0 || _coins < amount) return false;

            _coins -= amount; 
            CoinsBalanceChangedEvent.Execute(_coins);
            return true;
        }
        
        public void IncreaseServedCustomers()
        {
            ServedCustomersAmountChangedEvent.Execute(++_servedCustomersAmount);
        }
        
        public void AddPlant(string plantType)
        {
            if (string.IsNullOrEmpty(plantType)) throw new Exception(nameof(plantType));
            
            _availablePlants[plantType] = _availablePlants.TryGetValue(plantType, out var plant) ? plant + 1 : 1;
            PlantWasAddedEvent.Execute((plantType, amount: 1));
            
            DataAddedEvent?.Execute(plantType);
        }

        public bool TryRemovePlant(string plantType)
        {
            if (!_availablePlants.ContainsKey(plantType) || _availablePlants[plantType] == 0) return false;

            _availablePlants[plantType] -= 1;

            PlantWasRemovedEvent.Execute((plantType, amount: 1));
            return true;
        }
        
        public int GetPlantsCount(string key)
        {
            return _availablePlants.GetValueOrDefault(key);
        }
        
        public bool CheckPlantContains(string plantType)
        {
            return _availablePlants.ContainsKey(plantType) && _availablePlants[plantType] > 0;
        }

        public void AddEssence(string id)
        {
            _availableEssences.Add(id);
            
            DataAddedEvent?.Execute(id);
        }

        public void AddUniqItem(string id)
        {
            _uniqItems.Add(id);
            
            DataAddedEvent?.Execute(id);
        }
        
        public void SetCraftedPotion(string potionID)
        {
            _craftedPotion = potionID;
        }
    }
}