using System;
using _CodeBase.Garden;
using _CodeBase.Hall;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
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
    public sealed class GameplayService : MonoBehaviour, IGameState
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly TimeSpan GameTime = new(0, 0, minutes: 2, 0);
        
        private GameService _gameService;
        private InputManager _inputManager;
        private GameConfigProvider _gameConfigProvider;

        private readonly ReactiveCommand _statsChangedEvent = new();
        
        public static GameplayService Instance { get; private set; }

        public IReactiveCommand<Unit> StatsChangedEvent => _statsChangedEvent;
        public Timer GameTimer { get; private set; }
        public GameScene PreviousGameScene { get; private set; } = GameScene.None;
        public GameScene CurrentGameScene { get; private set; } = GameScene.None;
        public GameplayData Data { get; private set; } = new();
        public Camera Camera { get; private set; }
        public GameplayUIBinder UI { get; private set; }
        public GameObject CurrentGameSceneMap { get; private set; }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Inject]
        public void Init(GameService gameService, InputManager inputManager, GameplayUIBinder uiBinder, GameConfigProvider gameConfigProvider)
        {
            Instance = this;
            _gameConfigProvider = gameConfigProvider;
            UI = uiBinder;
            _inputManager = inputManager;
            _gameService = gameService;
        }

        public async void Enter()
        {
            FillGameplayData();
            
            Camera = Camera.main;
            UI.MainCanvas.sortingLayerName = "UI";
            UI.MainCanvas.worldCamera = Camera;
            
            UI.HudUI.Bind(this);
            _inputManager.IntractableInputFlag = true;
            _inputManager.SetCamera(Camera);
            GameTimer = new Timer();
            GameTimer.Run(GameTime);
            ObserveGameEnd().Forget();


            await GoToHallLac();
        }

        private void FillGameplayData()
        {
            foreach (var staticPlantSeeds in _gameConfigProvider.StaticPlantsForLanding)
            {
                Data.AddPlantsToLandingPool(staticPlantSeeds, GameplayData.InfinityLandingValue);
            }

            foreach (var staticEssence in _gameConfigProvider.StaticEssences)
            {
                Data.AddEssence(staticEssence);
            }

            Data.AddUniqItem(_gameConfigProvider.MixerUniqId);
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
        
        public void UpdateCustomerInfo(float loyalty)
        {
            UI.HudUI.UpdateCustomerIndicator(loyalty);
        }

        public void DisableAllUI()
        {
            UI.PotionUI.gameObject.SetActive(false);
            UI.GardenUI.gameObject.SetActive(false);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async UniTask SwitchLocation<TState>(GameScene scene) where TState : class, IGameState
        {
            DisableAllUI();
            await _gameService.TryLoadScene(scene, asAdditive: true);
            PreviousGameScene = CurrentGameScene;
            CurrentGameScene = scene;
            _gameService.GameStateMachine.Enter<TState>(beforeEnterAction: () => { CurrentGameSceneMap = (_gameService.GameStateMachine.CurrentGameState as GameplaySceneState)?.SceneMap; });
        }

        private async UniTaskVoid ObserveGameEnd()
        {
            await UniTask.WaitUntil(() => GameTimer.IsTimeUp);
        }
    }
}