using _CodeBase.Customers;
using _CodeBase.Garden;
using _CodeBase.Hall;
using _CodeBase.Infrastructure.DI;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Input;
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
        [SerializeField] private GameplayCursor _cursor;
        [SerializeField] private GameConfigProvider _gameConfigProvider;
        [SerializeField] private ShopConfigurationProvider _shopConfigurationProvider;
        [SerializeField] private GameplayConfig _gameplayConfig;
        
        
        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureGeneralSystems(builder);
            
            builder.RegisterInstance(_gameService);
            builder.RegisterInstance(_inputManager);
            builder.RegisterInstance(_cursor);
            builder.RegisterInstance(_gameConfigProvider);
            builder.RegisterInstance(_shopConfigurationProvider);
            builder.RegisterInstance(_gameplayConfig);
            
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