using System.Linq;
using _CodeBase.Customers._Data;
using _CodeBase.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _CodeBase.Customers
{
    public sealed class CustomerFetcher : MonoBehaviour
    {
        [SerializeField] private CustomersConfiguration _configuration;
        [SerializeField] private Customer _customer;
        
        public Customer GetNextCustomer()
        {
            return BuildCustomer();
        }

        private Customer BuildCustomer()
        {
            return _customer.Init
            (
                _configuration.Sprites.GetRandom(),
                _configuration.Orders.GetRandom(),
                _configuration.CustomerInfos.GetRandom()
            );
        }
    }
}