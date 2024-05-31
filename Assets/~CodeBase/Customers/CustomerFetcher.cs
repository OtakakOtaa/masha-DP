using System.Collections.Generic;
using System.Linq;
using _CodeBase.Customers._Data;
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
        [SerializeField] private int _noRepeatableIndex = 4;


        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        
        private readonly Queue<string> _ordersRecordsList = new();
        private readonly Queue<string> _customersInfoRecordsList = new();
        private readonly Queue<string> _customerVisualRecordsList = new();

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
            var data = await GetNextWithSimpleIDDel<CustomerInfo>(_customersInfoRecordsList, _gameConfigProvider.UniqCustomerInfo);
            var order = await GetNextOrder();
            
            return _customer.Init(visual, order, data);
        }
        
        private async UniTask<Order> GetNextOrder()
        {
            var order = _gameConfigProvider.GetRandomBasedOnWeight<Order>();
            if ((_ordersRecordsList.Contains(order.RequestedItemID) && _gameConfigProvider.UniqOrderCount > 1f) || (_availableOrdersMask[order.ID] is false))
            {
                await UniTask.Yield();
                return await GetNextOrder();
            }

            if (_ordersRecordsList.Count >= _noRepeatableIndex || _ordersRecordsList.Count >= _gameConfigProvider.UniqOrderCount)
            {
                _ordersRecordsList.Dequeue();
            }
            
            _ordersRecordsList.Enqueue(order.RequestedItemID);

            return order;
        }
        
        private async UniTask<TType> GetNextWithSimpleIDDel<TType>(Queue<string> recordList, int maxOriginalCapacity) where TType : PollEntity
        {
            var item = _gameConfigProvider.GetRandomBasedOnWeight<TType>();
            if (recordList.Contains(item.ID) && maxOriginalCapacity > 1f)
            {
                await UniTask.Yield();
                return await GetNextWithSimpleIDDel<TType>(recordList, maxOriginalCapacity);
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