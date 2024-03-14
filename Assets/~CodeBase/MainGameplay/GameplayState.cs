using System;
using _CodeBase.Garden;
using _CodeBase.Hall;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using _CodeBase.Input.Manager;
using _CodeBase.MainMenu;
using _CodeBase.Potion;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.MainGameplay
{
    public sealed partial class GameplayState : MonoBehaviour, IGameState
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly TimeSpan GameTime = new(0, 0, minutes: 2, 0);
        
        private GameService _gameService;
        private GameHudBinder _gameHudBinder;
        private InputManager _inputManager;

        private readonly ReactiveCommand _statsChangedEvent = new();
        public IReactiveCommand<Unit> StatsChangedEvent => _statsChangedEvent;
        

        public Timer GameTimer { get; private set; }
        public GameScene PreviousGameScene { get; private set; } = GameScene.None;
        public GameScene CurrentGameScene { get; private set; } = GameScene.None;
        public GameplayData Data { get; private set; } = new();
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Inject]
        public void Init(GameService gameService, GameHudBinder gameHudBinder, InputManager inputManager)
        {
            _inputManager = inputManager;
            _gameHudBinder = gameHudBinder;
            _gameService = gameService;
        }

        public async void Enter()
        {
            _gameService.GameplayService.gameplayState = this;

            _gameHudBinder.Bind(this);
            _inputManager.IntractableInputFlag = true;
            _inputManager.SetCamera(Camera.main);
            GameTimer = new Timer();
            GameTimer.Run(GameTime);
            ObserveGameEnd().Forget();


            await GoToHallLac();
        }

        public void OnDestroy()
        {
            _gameService.GameplayService.gameplayState = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async UniTask GoToPotionLac()
        {
            await SwitchLocation<GameplayPotionState>(GameScene.Laboratory);
        }

        public async UniTask GoToHallLac()
        {
            await SwitchLocation<GameplayHallState>(GameScene.Hall);
        }

        public async UniTask GoToGardenLac()
        {
            await SwitchLocation<GameplayGardenState>(GameScene.Garden);
        }

        public async UniTask GoToMainMenu()
        {
            await _gameService.TryLoadScene(GameScene.MainMenu);
            _gameService.GameStateMachine.Enter<MainMenuGameState>(ignoreExit: true);
        }

        public void AddMoney(int money)
        {
            Data.Coins += money;
            _statsChangedEvent.Execute();
        }

        public void IncrementCustomerStat()
        {
            Data.ServedCustomersAmount += 1;
            _statsChangedEvent.Execute();
        }

        public void AddGrownPlant(PlantType plantType)
        {
            Data.AddPlant(plantType);
        }
        
        public void UpdateCustomerInfo(float loyalty)
        {
            _gameHudBinder.UpdateCustomerIndicator(loyalty);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async UniTask SwitchLocation<TState>(GameScene scene) where TState : class, IGameState
        {
            await _gameService.TryLoadScene(scene, asAdditive: true);
            PreviousGameScene = CurrentGameScene;
            CurrentGameScene = scene;
            _gameService.GameStateMachine.Enter<TState>();
        }

        private async UniTaskVoid ObserveGameEnd()
        {
            await UniTask.WaitUntil(() => GameTimer.IsTimeUp);
        }
    }
}