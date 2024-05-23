using System;
using _CodeBase.Infrastructure.DI;
using _CodeBase.MainGameplay;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace _CodeBase.DiScopes
{
    public sealed class GameplayDiScope : LifetimeScope 
    {
        [SerializeField] private GameplayUIBinder _uiBinder;
        [SerializeField] private GameplayService _gameplayService;
        
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_uiBinder);
            builder.RegisterInstance(_gameplayService);
            
            Parent.Container.Resolve<DiContainer>().Scope = this;
        }
        
        protected override void OnDestroy()
        {
            var container = Parent.Container.Resolve<DiContainer>();
            if(container == null) return;
            
            container.Scope = Parent;
        }
    }
}