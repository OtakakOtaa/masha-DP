using System;
using System.Collections.Generic;
using _CodeBase.Garden;
using UnityEngine;

namespace _CodeBase.MainGameplay
{
    [Serializable] public sealed class GameplayData
    {
        [SerializeField] private int _coins;
        [SerializeField] private int _servedCustomersAmount;

        private readonly Dictionary<PlantType, int> _availablePlants = new();

        public int Coins
        {
            get => _coins;
            set => _coins = value;
        }

        public int ServedCustomersAmount
        {
            get => _servedCustomersAmount;
            set => _servedCustomersAmount = value;
        }

        public void AddPlant(PlantType plantType)
        {
            _availablePlants[plantType] = _availablePlants.TryGetValue(plantType, out var plant) ? plant + 1 : 1;
        }

        public void TryRemovePlant(PlantType plantType)
        {
            if (!_availablePlants.ContainsKey(plantType) || _availablePlants[plantType] == 0) return;

            _availablePlants[plantType] -= 1;
        }

        public bool CheckPlantContains(PlantType plantType)
        {
            return _availablePlants.ContainsKey(plantType) && _availablePlants[plantType] > 0;
        }
    }
}