using System.Threading;
using _CodeBase.Customers;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using VContainer;
using Timer = _CodeBase.MainGameplay.Timer;

namespace _CodeBase.Hall
{
    public sealed class GameplayHallState : GameplaySceneState
    {
        [SerializeField] private PotionDummy _potionDummy;
        [SerializeField] private CustomerFetcher _customerFetcher;
        [SerializeField] private DialogueBubble _dialogueBubble;
        

        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        
        
        private Customer _activeCustomer;
        private readonly Timer _customerExpectationTimer = new();
        
        public bool _freeHallFlag;
        private bool _customerExpectationFlag;
        private bool _correctCraftedPotionFlag;
        private float _customerLoyalty;
        private bool _tookOrderFlag;
        private bool _potionDeliveryFlag = false;


        
        protected override async void OnFirstEnter()
        {
            _potionDummy.gameObject.SetActive(false);
            _customerFetcher.Init();
            
            GameService.GameUpdate.Subscribe(_ => OnUpdate()).AddTo(destroyCancellationToken);
            _gameplayService.Data.CreatedPotionEvent.Subscribe(_ => UpdateCraftedPotion()).AddTo(destroyCancellationToken);
            _gameplayService.BindGameEndAdditionalRestriction(() => _tookOrderFlag);
            
            await UniTask.WaitForSeconds(GameplayConfig.Instance.FirstCustomerEnterDelay, cancellationToken: destroyCancellationToken);
            while (StateLiveToken.IsCancellationRequested is false && _gameplayService.GameTimer.IsTimeUp is false)
            {
                await LaunchCustomer(StateLiveToken);
                await ExecuteLeaveCustomer(StateLiveToken, _correctCraftedPotionFlag, !(_gameplayService.GameTimer.IsTimeUp && _potionDeliveryFlag is false));
                await UniTask.WaitForSeconds(GameplayConfig.Instance.GetDelayBetweenCustomers(_gameplayService.GameTimer.TimeRatio), cancellationToken: StateLiveToken);
            }
        }

        protected override void OnEnter() { }

        protected override void OnExit() { }

        
        
        private void OnUpdate()
        {
            if (!_customerExpectationFlag) return;
            
            _customerLoyalty = _customerExpectationFlag ? 1f - _customerExpectationTimer.TimeRatio : 1;
            if (_customerLoyalty <= 0.01f) _customerLoyalty = 0f;
            _gameplayService.UpdateCustomerInfo(_customerLoyalty);
        }

        private void UpdateCraftedPotion()
        {
            if (string.IsNullOrEmpty(_gameplayService.Data.CraftedPotion)) return;
            
            _potionDummy.gameObject.SetActive(true);
            _potionDummy.Init(_gameConfigProvider.GetByID<PotionConfig>(_gameplayService.Data.CraftedPotion));
        } 
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        [Button(nameof(LaunchCustomer))]
        private async UniTask LaunchCustomer(CancellationToken token)
        {
            _correctCraftedPotionFlag = false;
            _freeHallFlag = false;
            _customerExpectationFlag = false;
            _tookOrderFlag = false;
            _potionDeliveryFlag = false;
            
            _activeCustomer = await _customerFetcher.GetNextCustomer();
            await ExecuteCustomerEntering(token);
            if (_gameplayService.GameTimer.IsTimeUp) return;
            
            _potionDeliveryFlag = false;
            var subToPotionDelivery = _activeCustomer.GetPotionEvent.First().Subscribe(_ => _potionDeliveryFlag= true);
            await UniTask.WaitUntil(() => (_customerExpectationFlag && (_customerLoyalty <= 0.01f)) || _potionDeliveryFlag, cancellationToken: token);
            subToPotionDelivery?.Dispose();
            
            _correctCraftedPotionFlag = _potionDeliveryFlag && _activeCustomer.Order.RequestedItemID == _potionDummy.ID; 
            
            if (_potionDeliveryFlag)
            {
                _potionDummy.gameObject.SetActive(false);
                _gameplayService.Data.SetCraftedPotion(null);
            }
            else
            {
                if (_gameplayService.CurrentGameScene != GameScene.Hall)
                {
                    _gameplayService.UI.HudUI.ShowClientLeaveIndicator().Forget();
                }
            }
            
            if (_correctCraftedPotionFlag)
            {
                _gameplayService.Data.AddCustomerCoinToBalance(_activeCustomer.Order.Reward);
            }
        }

        [Button(nameof(ExecuteLeaveCustomer))]
        private async UniTask ExecuteLeaveCustomer(CancellationToken token, bool isCorrectPotion, bool needBubble = true)
        {
            if (needBubble)
            {
                await UniTask.WaitForSeconds(GameplayConfig.Instance.BubblePassDelay, cancellationToken: token);
                _dialogueBubble.Activate(showButtons: false);
                await _dialogueBubble.ExecuteMessFill(isCorrectPotion ? _activeCustomer.GoodFarewellWord.Mess : _activeCustomer.BadFarewellWord.Mess, token);
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                _dialogueBubble.Deactivate();
            }
            
            _customerExpectationFlag = false;
            await _activeCustomer.ExecuteLeaving(token);
            
            _activeCustomer = null;
            _freeHallFlag = true;
            _tookOrderFlag = false;
            _potionDeliveryFlag = false;
        }

        private async UniTask ExecuteCustomerEntering(CancellationToken token)
        {
            await _activeCustomer.ExecuteEntering(token);
            _gameplayService.UpdateCustomerInfo(1f);
            _tookOrderFlag = false;
            
            await UniTask.WaitUntil(() => ActiveFlag, cancellationToken: token);
            await UniTask.WaitForSeconds(GameplayConfig.Instance.BubblePassDelay, cancellationToken: token);
            _dialogueBubble.Activate(_activeCustomer.Order).Forget();
            await UniTask.WaitUntil(() => _dialogueBubble.BubbleOpenedFlag is false || _gameplayService.GameTimer.IsTimeUp, cancellationToken: token);
            if (_gameplayService.GameTimer.IsTimeUp)
            {
                _dialogueBubble.Deactivate();
                return;
            }
            _tookOrderFlag = true;
            
            _customerExpectationFlag = false;
            if (_activeCustomer.Order.NeedWaitTime)
            {
                _customerExpectationTimer.RunWithDuration(_activeCustomer.Order.TimeToReady);
                _customerExpectationFlag = true;   
            }
            
            _customerLoyalty = 1f;
        }
    }
} 