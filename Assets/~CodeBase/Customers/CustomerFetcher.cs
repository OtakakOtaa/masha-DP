using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Customers._Data;
using _CodeBase.DATA;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _CodeBase.Customers
{
    public sealed class CustomerFetcher : MonoBehaviour
    {
        [SerializeField] private Customer _customer;
        [SerializeField] private int _noRepeatableIndex = 3;
        
        private const int ReRandomMaxCount = 50;


        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        
        private readonly Queue<string> _ordersRecordsList = new();
        private readonly Queue<string> _customersInfoRecordsList = new();
        private readonly Queue<string> _customerVisualRecordsList = new();
        private readonly Queue<string> _customerGoodFarewellList = new();
        private readonly Queue<string> _customerBadFarewellList = new();

        private Dictionary<string, bool> _availableOrdersMask;


        public void Init()
        {
            _availableOrdersMask = new Dictionary<string, bool>();
            
            var orders = _gameConfigProvider.Orders.ToArray();
            var accessedComponentsIDs = _gameplayService.Data.Seeds.Concat(_gameplayService.Data.AccessibleEssences);

            if (_gameplayService.Data.UniqItems.Contains(_gameConfigProvider.MixerUniqId))
            {
                accessedComponentsIDs = accessedComponentsIDs.Concat(new[] { _gameConfigProvider.MixerUniqId });
            }
            
            foreach (var order in orders)
            {
                var potion = _gameConfigProvider.GetByID<PotionConfig>(order.RequestedItemID);
                
                var canConceivablyCreated = potion.Compound.All(c => accessedComponentsIDs.Contains(c.ID));
                _availableOrdersMask[order.ID] = canConceivablyCreated;
            }
        }
        
        public async UniTask<Customer> GetNextCustomer()
        {
            var visual = await GetNextWithSimpleIDDel<CustomerVisual>(_customerVisualRecordsList, _gameConfigProvider.UniqVisualCount);
            var data = await GetNextWithSimpleIDDel<CustomerInfo>(_customersInfoRecordsList, _gameConfigProvider.UniqCustomerInfoCount);
            
            var goodFarewell = await GetNextWithSimpleIDDel(_customerGoodFarewellList, _gameConfigProvider.GoodCustomerFarewells.Count(), 
                () => _gameConfigProvider.GetRandomBasedOnWeightFarewell(true));
            
            var badFarewell = await GetNextWithSimpleIDDel(_customerBadFarewellList, _gameConfigProvider.BadCustomerFarewells.Count(), 
                () => _gameConfigProvider.GetRandomBasedOnWeightFarewell(false));
            
            var order = await GetNextOrder();
            
            return _customer.Init(visual, order, data, goodFarewell, badFarewell);
        }
        
        private async UniTask<Order> GetNextOrder(int callCounter = 0)
        {
            var order = _gameConfigProvider.GetRandomBasedOnWeight<Order>();
            if ((_ordersRecordsList.Contains(order.RequestedItemID) && _gameConfigProvider.UniqOrderCount > 1f) || (_availableOrdersMask[order.ID] is false) && callCounter < ReRandomMaxCount)
            {
                await UniTask.Yield();
                return await GetNextOrder(++callCounter);
            }

            if (_ordersRecordsList.Count >= _noRepeatableIndex || _ordersRecordsList.Count >= _gameConfigProvider.UniqOrderCount)
            {
                _ordersRecordsList.Dequeue();
            }
            
            _ordersRecordsList.Enqueue(order.RequestedItemID);

            return order;
        }
        
        private async UniTask<TType> GetNextWithSimpleIDDel<TType>(Queue<string> recordList, int maxOriginalCapacity, Func<TType> randomItemFunc = null, int callCounter = 0) where TType : PollEntity
        {
            var item = randomItemFunc == null ? _gameConfigProvider.GetRandomBasedOnWeight<TType>() : randomItemFunc.Invoke();
            if (recordList.Contains(item.ID) && maxOriginalCapacity > 1f && callCounter < ReRandomMaxCount)
            {
                await UniTask.Yield();
                return await GetNextWithSimpleIDDel<TType>(recordList, maxOriginalCapacity, randomItemFunc, callCounter: ++callCounter);
            }

            if (recordList.Count >= _noRepeatableIndex || recordList.Count >= maxOriginalCapacity)
            {
                recordList.Dequeue();
            }
            
            recordList.Enqueue(item.ID);

            return item;
        }
    }
}