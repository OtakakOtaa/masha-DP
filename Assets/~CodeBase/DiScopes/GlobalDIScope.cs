using _CodeBase.Garden;
using _CodeBase.Hall;
using _CodeBase.Infrastructure.DI;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using _CodeBase.MainMenu;
using _CodeBase.Potion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _CodeBase.DiScopes
{
    public sealed class GlobalDiScope : LifetimeScope
    {
        [SerializeField] private GameService _gameService;
        [SerializeField] private InputManager _inputManager;
        
        
        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureGeneralSystems(builder);
            builder.RegisterInstance(_gameService);
            builder.RegisterInstance(_inputManager);
            
            ConfigureGameStateMachine(builder);
        }

        private void ConfigureGeneralSystems(IContainerBuilder builder)
        {
            builder.Register<DiContainer>(Lifetime.Singleton).WithParameter(this);
        }

        private void ConfigureGameStateMachine(IContainerBuilder builder)
        {
            var finalStates = new[]
            {
                typeof(GameService),
                typeof(MainMenuGameState),
                typeof(GameplayService),
                typeof(GameplayHallState),
                typeof(GameplayGardenState),
                typeof(GameplayPotionState)
            };

            builder.Register<GlobalStateMachine>(Lifetime.Singleton).WithParameter(finalStates);
        }
    }
}