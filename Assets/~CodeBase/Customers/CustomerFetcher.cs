using System.Collections.Generic;
using _CodeBase.Customers._Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CodeBase.Customers
{
    public sealed class CustomerFetcher : MonoBehaviour
    {
        [FormerlySerializedAs("_gameDataProvider")] [SerializeField] private GameConfigProvider _gameConfigProvider;
        [SerializeField] private Customer _customer;
        [SerializeField] private int _noRepeatableIndex = 4;
        
        
        private readonly Queue<string> _ordersRecordsList = new();
        private readonly Queue<string> _customersInfoRecordsList = new();
        private readonly Queue<string> _customerVisualRecordsList = new();
        
        
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
            if (_ordersRecordsList.Contains(order.RequestedItemID))
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
            if (recordList.Contains(item.ID))
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