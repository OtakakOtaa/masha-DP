using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _CodeBase.MainMenu
{
    public sealed class MainMenuGameState : IGameState
    {
        private readonly GameService _gameService;
        private readonly InputManager _inputManager;
        private MainMenuBinder _mainMenuBinder;
        
        
        public MainMenuGameState(GameService gameService, InputManager inputManager)
        {
            _gameService = gameService;
            _inputManager = inputManager;
        }

        
        public void Enter()
        {
            _mainMenuBinder = UnityEngine.Object.FindObjectOfType<MainMenuBinder>();
            _mainMenuBinder.SetHandler(mainMenuGameState: this);
            _inputManager.IntractableInputFlag = false;
            _inputManager.SetCamera(Camera.main);
        }

        public async UniTaskVoid StartGame()
        {
            await _gameService.TryLoadScene(GameScene.Gameplay);
            _gameService.GameStateMachine.Enter<GameplayService>();
        }
    }
}