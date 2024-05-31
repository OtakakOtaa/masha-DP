using System.Collections.Generic;
using System.Linq;
using _CodeBase.Customers._Data;
using _CodeBase.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace _CodeBase.Customers
{
    [CreateAssetMenu(fileName = nameof(CustomersConfiguration), menuName = ConfigPathOrigin.Gameplay + "/" + nameof(CustomersConfiguration))]
    public class CustomersConfiguration : ScriptableObject
    {
        [TabGroup("orders")]
        [ListDrawerSettings(ShowPaging = false, Expanded = true, ShowIndexLabels = true, ShowItemCount = true, ListElementLabelName = "_requestedItemID")]
        [SerializeField] private List<Order> _orders;

        [TabGroup("visual")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<CustomerVisual> _customerVisuals;

        [TabGroup("info")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<CustomerInfo> _customerInfos;

        [TabGroup("last_words")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<CustomerFarewellWord> _farewellWords;
        
        public IEnumerable<Order> Orders => _orders;
        public IEnumerable<CustomerVisual> CustomerVisuals => _customerVisuals;
        public IEnumerable<CustomerInfo> CustomerInfos => _customerInfos;
        public List<CustomerFarewellWord> FarewellWords => _farewellWords;
        

#if UNITY_EDITOR

        [TabGroup("orders")] [Button("ClearAllTabs")]
        public void ClearAllTabs()
        {
            foreach (var order in _orders)
            {
                order.Set(order.Message.Replace("\r\n", " "), order.ConcreteMessage.Replace("\r\n", " "));
            }
        }
        
        [TabGroup("orders")] [Button("AutoFillIDs")]
        private void AutoFillIDsForOrder()
        {
            _orders.Select((i, i1) => (i, i1)).ForEach(i => i.i.SetID_EDITOR($"[ORDER] - {i.i.RequestedItemID} #{i.i1}"));
        }
        
        
        [TabGroup("visual")] [Button("AutoFillIDs")]
        private void AutoFillIDsForVisual()
        {
            _customerVisuals.Select((i, i1) => (i, i1)).ForEach(i => i.i.SetID_EDITOR($"[VISUAL] - {i.i.Sprite.name} #{i.i1}"));
        }
        
        [TabGroup("info")] [Button("AutoFillIDs")]
        private void AutoFillIDsForInfo()
        {
            _customerInfos.Select((i, i1) => (i, i1)).ForEach(i => i.i.SetID_EDITOR($"[INFO] - {i.i.Name} #{i.i1}"));
        }
        
        [TabGroup("last_words")] [Button("AutoFillIDs")]
        private void AutoFillIDsForLastWords()
        {
            _farewellWords.Select((i, i1) => (i, i1)).ForEach(i => i.i.SetID_EDITOR($"[FAREWELL] - #{i.i1}"));
        }

#endif
    }
}