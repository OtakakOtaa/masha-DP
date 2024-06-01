using System;
using System.Linq;
using _CodeBase.Customers;
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
        public static GameplayService Instance { get; private set; }

        
        [Inject] private GameService _gameService;
        [Inject] private InputManager _inputManager;
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private ShopConfigurationProvider _shopConfigurationProvider;

        private readonly ReactiveCommand _statsChangedEvent = new();
        
        [Inject] public GameplayUIContainer UI { get; private set; }
        public IReactiveCommand<Unit> StatsChangedEvent => _statsChangedEvent;
        public Timer GameTimer { get; private set; }
        public GameScene PreviousGameScene { get; private set; } = GameScene.None;
        public GameScene CurrentGameScene { get; private set; } = GameScene.None;
        public GameplayData Data { get; private set; } = new();
        public Camera Camera { get; private set; }
        public GameObject CurrentGameSceneMap { get; private set; }
        public int EarnedMoney { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            Instance = this;
        }
        
        
        public async void Enter()
        {
            FillGameplayData();
            
            Camera = Camera.main;
            UI.MainCanvas.sortingLayerName = "UI";
            UI.MainCanvas.worldCamera = Camera;
            
            _inputManager.IntractableInputFlag = true;
            _inputManager.SetCamera(Camera);
            
            UI.HudUI.gameObject.SetActive(false);
            InitAndOpenShop();
            await UniTask.WaitUntil(() => UI.ShopUI.ReturnToSignalFlag || UI.ShopUI.ContinueSignalFlag);
            
            if (UI.ShopUI.ReturnToSignalFlag)
            {
                await GoToMainMenu();
                return;
            }
            if (UI.ShopUI.ContinueSignalFlag)
            {
                GameTimer = new Timer();
                GameTimer.RunWithDuration(GameplayConfig.Instance.DayDuration);
                HookGameEnd().Forget();
                
                UI.HudUI.Bind(this);
                UI.HudUI.gameObject.SetActive(true);
                await GoToHallLac();
                return;
            }
        }

        private void FillGameplayData()
        {
            foreach (var item in _gameService.PersistentGameData.Items)
            {
                var type = _gameConfigProvider.TryDefineTypeByID(item);

                switch (type)
                {
                    case UniqItemsType.Essence:
                    {
                        if (item == _gameConfigProvider.MixerUniqId)
                        {
                            Data.AddUniqItem(item);
                        }
                        else
                        {
                            Data.AddEssence(item);
                        }
                    }
                        break;
                    case UniqItemsType.Plant:
                        Data.AddPlantsToLandingPool(item, GameplayData.InfinityLandingValue);
                        break;
                    default:
                        throw new Exception(nameof(FillGameplayData));
                }
            }
            
            Data.ChangeGlobalCoinBalance(_gameService.PersistentGameData.Coins);
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

        public void InitAndOpenShop()
        {
            DisableAllUI();
            var nonPurchasedKeys = _shopConfigurationProvider.ShopItemData
                .Select(d => d.ID)
                .Except(_gameService.PersistentGameData.Items)
                .ToArray();
            
            
            UI.ShopUI.gameObject.SetActive(true);
            UI.ShopUI.Init(nonPurchasedKeys);

            UI.ShopUI.BuyBtnPressEvent.Subscribe(HandleBuyItemRequest).AddTo(destroyCancellationToken);
        }
        
        public void UpdateCustomerInfo(float loyalty)
        {
            UI.HudUI.UpdateCustomerIndicator(loyalty);
        }
        
        public void DisableAllUI()
        {
            UI.PotionUI.gameObject.SetActive(false);
            UI.GardenUI.gameObject.SetActive(false);
            UI.ShopUI.gameObject.SetActive(false);
            UI.DayResultsUI.gameObject.SetActive(false);
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

        private async UniTaskVoid HookGameEnd()
        {
            await UniTask.WaitUntil(() => GameTimer.IsTimeUp);
            UI.DayResultsUI.gameObject.SetActive(true);
            UI.DayResultsUI.Init(Data.EarnedCoins);
            await UniTask.WaitUntil(() => UI.DayResultsUI.ContinueEventFlag, cancellationToken: destroyCancellationToken);

            SaveGameplayData();
            GoToMainMenu();
        }
        
        private void HandleBuyItemRequest(string id)
        {
            var data = _shopConfigurationProvider.GetDataByID(id);
            if (data == null) throw new Exception(nameof(HandleBuyItemRequest));
            
            var operationFlag = Data.TryWithdrawGlobalCoins(data.Cost);
            if (!operationFlag) return;


            if (data.ID == _gameConfigProvider.MixerUniqId)
            {
                Data.AddUniqItem(data.ID);
                return;
            }
            
            var type = _gameConfigProvider.TryDefineTypeByID(data.ID);
            switch (type)
            {
                case UniqItemsType.Essence:
                    Data.AddEssence(data.ID);
                    break;
                case UniqItemsType.Plant:
                    Data.AddPlantsToLandingPool(data.ID, GameplayData.InfinityLandingValue);
                    break;
                case UniqItemsType.Booster:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SaveGameplayData()
        {
            _gameService.PersistentGameData.UpdateItems(Data.Seeds.Union(Data.AccessibleEssences).Union(Data.UniqItems));

            var hasPursuance = _gameService.PersistentGameData.Coins > Data.GlobalCoins;
            if (hasPursuance)
            {
                _gameService.PersistentGameData.Withdraw(_gameService.PersistentGameData.Coins - Data.GlobalCoins);
            }

            var hasEarnedCoins = Data.EarnedCoins > 0;
            if (hasEarnedCoins)
            {
                _gameService.PersistentGameData.Deposit(Data.EarnedCoins);
            }
            
            _gameService.PersistentGameData.IncreaseDay();

            _gameService.SaveGameData();
        }
    }
}