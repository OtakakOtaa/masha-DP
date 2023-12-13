using _CodeBase._GameCycle.States;
using _CodeBase._GameCycle.States.Game;
using _CodeBase._GameCycle.States.Game.Garden;
using _CodeBase._GameCycle.States.Game.Potion;
using _CodeBase._GameCycle.States.MainMenu;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.DI;
using _CodeBase.Infrastructure.GameStructs.FSM;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _CodeBase._GameCycle.ScopeRegister
{
    public sealed class GlobalDiScope : LifetimeScope
    {
        [SerializeField] private ScenesConfiguration _scenesConfiguration;

        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureGameStateMachine(builder);
            ConfigureGeneralSystems(builder);
        }
        private void ConfigureGeneralSystems(IContainerBuilder builder)
        {
            builder.Register<DiResolver>(Lifetime.Singleton).WithParameter(this); 
            builder.RegisterInstance(_scenesConfiguration);
        }
        
        private void ConfigureGameStateMachine(IContainerBuilder builder)
        {
            var finalStates = new TypesCollector
            (
                typeof(BootstrapGameState),
                typeof(MainMenuGameState),
                typeof(GameplayRootState)
            );

            builder.Register<GameStatesResolver>(Lifetime.Singleton).WithParameter(finalStates);
            builder.Register<GlobalStateMachine>(Lifetime.Singleton).WithParameter(finalStates);
        }

    }
}