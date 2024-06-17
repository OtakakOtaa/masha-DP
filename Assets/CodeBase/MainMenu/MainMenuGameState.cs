using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using CodeBase.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _CodeBase.MainMenu
{
    public sealed class MainMenuGameState : IGameState
    {
        [Inject] private readonly GameService _gameService;
        [Inject] private readonly InputManager _inputManager;
        [Inject] private readonly AudioService _audioService;
        
        private MainMenuBinder _mainMenuBinder;
        
        public async void Enter()
        {
            _audioService.ResetAmbience();
            _audioService.ChangeAmbience("sound_main_A");
            _audioService.ContinueAmbience();
            
            _mainMenuBinder = Object.FindObjectOfType<MainMenuBinder>();
            _mainMenuBinder.SetHandler(mainMenuGameState: this);
            _inputManager.IntractableInputFlag = false;
            _inputManager.SetCamera(Camera.main);
            
            await _gameService.Curtain.PlayAppears();
        }

        public async UniTaskVoid StartGame()
        {
            await _gameService.Curtain.PlayAppears();
            await _gameService.TryLoadScene(GameScene.Gameplay);
            _gameService.GameStateMachine.Enter<GameplayService>();
        }
    }
}