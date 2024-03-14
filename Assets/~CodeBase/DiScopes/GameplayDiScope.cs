using _CodeBase.Infrastructure.DI;
using _CodeBase.MainGameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _CodeBase.DiScopes
{
    public sealed class GameplayDiScope : LifetimeScope 
    {
        [SerializeField] private GameHudBinder _gameHudBinder;
        
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameHudBinder);
            
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