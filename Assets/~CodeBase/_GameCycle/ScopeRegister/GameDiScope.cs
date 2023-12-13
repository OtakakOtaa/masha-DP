using _CodeBase.Customers;
using _CodeBase.Customers._Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _CodeBase._GameCycle.ScopeRegister
{
    public sealed class GameDiScope : LifetimeScope 
    {
        [SerializeField] private CustomerFetcher _customerFetcher;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_customerFetcher);
        }
    }
}