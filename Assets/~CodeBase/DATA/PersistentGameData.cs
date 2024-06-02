using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.DATA
{
    [Serializable] public sealed class PersistentGameData
    {
        [SerializeField] private int _day = 1;
        
        [ValueDropdown("@MashaEditorUtility.GetAllEssencesAndPlantsAndBoostersID()")]
        [SerializeField] private List<string> _items = new();
        [SerializeField] private int _coins;

        public static PersistentGameData Clone(PersistentGameData reference)
        {
            return new PersistentGameData
            {
                _day = reference._day,
                _items = reference.Items.ToList(),
                _coins = reference.Coins
            };
        } 
        
        
        public void AddItem(string item)
        {
            if (_items.Contains(item)) throw new Exception(nameof(PersistentGameData));
            
            _items.Add(item);
        }

        public void UpdateItems(IEnumerable<string> items)
        {
            _items = items.Distinct().ToList();
        }
        
        public void IncreaseDay()
        {
            _day++;
        }
        
        public void Deposit(int amount)
        {
            if (amount <= 0) throw new Exception(nameof(PersistentGameData));
            _coins += amount;
        }
        
        public void Withdraw(int amount)
        {
            if (amount <= 0 || amount > _coins) throw new Exception(nameof(PersistentGameData));
            
            _coins -= amount;
        }
        
        
        public int Coins => _coins;
        public int Day => _day;
        public IEnumerable<string> Items => _items.ToArray();
    }
}