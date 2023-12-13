using System.Collections.Generic;
using _CodeBase.Customers._Data;
using _CodeBase.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Customers
{
    [CreateAssetMenu(fileName = nameof(CustomersConfiguration), menuName = ConfigPathOrigin.Gameplay + "/" + nameof(CustomersConfiguration))]
    public class CustomersConfiguration : ScriptableObject
    {
        [TabGroup("orders")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<Order> _orders;

        [TabGroup("sprites")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<CustomerVisual> _customerVisuals;

        [TabGroup("C info")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [SerializeField] private List<CustomerInfo> _customerInfos;

        public IEnumerable<Order> Orders => _orders;
        public IEnumerable<CustomerVisual> Sprites => _customerVisuals;
        public IEnumerable<CustomerInfo> CustomerInfos => _customerInfos;
    }
}