using System;
using System.Linq;
using _CodeBase.Customers;
using _CodeBase.DATA;
using _CodeBase.Garden;
using _CodeBase.Hall;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using _CodeBase.Input.Manager;
using _CodeBase.MainMenu;
using _CodeBase.Potion;
using CodeBase.Audio;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.MainGameplay
{
    public sealed class GameplayService : MonoBehaviour, IExitableGameState
    {
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private string _ambienceName;
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _purchaseSFX;
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameplayService Instance { get; private set; }

        
        [Inject] private GameService _gameService;
        [Inject] private InputManager _inputManager;
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private ShopConfigurationProvider _shopConfigurationProvider;
        [Inject] private AudioService _audioService;
        
        private Func<bool> _gameEndRestrictions;
        
        [Inject] public GameplayUIContainer UI { get; private set; }
        public Timer GameTimer { get; private set; }
        public GameScene PreviousGameScene { get; private set; } = GameScene.None;
        public GameScene CurrentGameScene { get; private set; } = GameScene.None;
        public GameplayData Data { get; private set; } = new();
        public Camera UICamera { get; private set; }
        public GameObject CurrentGameSceneMap { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Awake()
        {
            Instance = this;
        }
        
        
        public async void Enter()
        {
            if (_gameService.GameStateMachine.PreviousGameState is not MainMenuGameState)
            {
                _audioService.ResetAmbience();
                _audioService.ChangeAmbience("sound_main_A");
                _audioService.ContinueAmbience();
            }
            
            InitMainData();
            DisableAllUI();
            FillGameplayData();
            InitInput();

            var isNotFirstEnter = _gameService.PersistentGameData.Day > 1; 
            if (isNotFirstEnter)
            {
                var returnFlag = await ShowAndHandleShop();
                if (returnFlag) return;
            }
            
            BoosterProcessor.ProcessBoosters(boosters: Data.UniqItems, _gameConfigProvider, Data);
            GameTimer = new Timer();
            GameTimer.RunWithDuration(GameSettingsConfiguration.Instance.DayDuration); 
            HookGameEnd().Forget();

            UI.HudUI.Bind(this);
            UI.HudUI.gameObject.SetActive(true);
            
            UI.StartDayUI.InitAndShow(_gameService.PersistentGameData.Day, needAnimation: false);
            await _gameService.Curtain.PlayDisappearance();
            await UniTask.WaitForSeconds(GameSettingsConfiguration.Instance.StartGameCurtainLifeDuration, cancellationToken: destroyCancellationToken);
            
            _audioService.HideAmbience(0.6f);
            await _gameService.Curtain.PlayAppears();
            await GoToHallLac();
            UI.StartDayUI.gameObject.SetActive(false);
            await _gameService.Curtain.PlayDisappearance().ContinueWith(() =>
            {
                _audioService.ResetAmbience();
                _audioService.ChangeAmbience(_ambienceName);
            });
        }

        public void Exit()
        {
            _audioService.StopAmbience();
        }

        private async UniTask<bool> ShowAndHandleShop()
        {
            UI.HudUI.gameObject.SetActive(false);
            InitAndOpenShop();
            await _gameService.Curtain.PlayDisappearance();
            
            await UniTask.WaitUntil(() => UI.ShopUI.ReturnToSignalFlag || UI.ShopUI.ContinueSignalFlag, cancellationToken: destroyCancellationToken);
            
            if (UI.ShopUI.ReturnToSignalFlag)
            {
                await GoToMainMenu();
                return true;
            }

            await _gameService.Curtain.PlayAppears();
            return false;
        }

        private void InitMainData()
        {
            _gameEndRestrictions = default;
            UICamera = Camera.main;
            UI.MainCanvas.sortingLayerName = "UI";
            UI.MainCanvas.worldCamera = _uiCamera;
        }

        private void InitInput()
        {
            _inputManager.IntractableInputFlag = true;
            _inputManager.SetCamera(UICamera);
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
            await _gameService.Curtain.PlayAppears();
            await _gameService.TryLoadScene(GameScene.MainMenu);
            _gameService.GameStateMachine.Enter<MainMenuGameState>(ignoreExit: true);
            await _gameService.Curtain.PlayAppears();
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
            UI.StartDayUI.gameObject.SetActive(false);
        }

        public void BindGameEndAdditionalRestriction(Func<bool> restriction)
        {
            _gameEndRestrictions += restriction;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                    case UniqItemsType.Booster:
                    Data.AddUniqItem(item);
                        break;
                    default:
                        throw new Exception(nameof(FillGameplayData));
                }
            }
            
            Data.ChangeGlobalCoinBalance(_gameService.PersistentGameData.Coins);
        } 
        
        private void InitAndOpenShop()
        {
            var nonPurchasedKeys = _shopConfigurationProvider.ShopItemData
                .Select(d => d.ID)
                .Except(_gameService.PersistentGameData.Items)
                .ToArray();
            
            
            UI.ShopUI.gameObject.SetActive(true);
            UI.ShopUI.Init(nonPurchasedKeys);

            UI.ShopUI.BuyBtnPressEvent.Subscribe(HandleBuyItemRequest).AddTo(destroyCancellationToken);
        }
        
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
            await UniTask.WaitUntil(() => GameTimer.IsTimeUp && (_gameEndRestrictions?.Invoke() is false), cancellationToken: destroyCancellationToken);

            UI.DayResultsUI.Init(Data.EarnedCoins);
            UI.DayResultsUI.Show();
            await UniTask.WaitUntil(() => UI.DayResultsUI.ContinueEventFlag, cancellationToken: destroyCancellationToken);

            SaveGameplayData();
            await _gameService.Curtain.PlayAppears();
            _gameService.GameStateMachine.Enter<StateForReload>();
        }
        
        private void HandleBuyItemRequest(string id)
        {
            var data = _shopConfigurationProvider.GetDataByID(id);
            if (data == null) throw new Exception(nameof(HandleBuyItemRequest));
            
            var operationFlag = Data.TryWithdrawGlobalCoins(data.Cost);
            if (!operationFlag) return;

            _audioService.PlayEffect(_purchaseSFX);
            
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
                    Data.AddUniqItem(data.ID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SaveGameplayData()
        {
            _gameService.PersistentGameData.UpdateItems(Enumerable.Union(Data.Seeds, Data.AccessibleEssences).Union(Data.UniqItems));

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