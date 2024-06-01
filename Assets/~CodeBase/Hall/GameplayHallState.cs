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
        private float _customerLoyalty;

        
        protected override async void OnFirstEnter()
        {
            _potionDummy.gameObject.SetActive(false);
            _customerFetcher.Init();
            
            GameService.GameUpdate.Subscribe(_ => OnUpdate()).AddTo(destroyCancellationToken);
            _gameplayService.Data.CreatedPotionEvent.Subscribe(_ => UpdateCraftedPotion()).AddTo(destroyCancellationToken);

            await UniTask.WaitForSeconds(GameplayConfig.Instance.FirstCustomerEnterDelay);
            LaunchCustomer(stateProcess.Token).Forget();
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
            _freeHallFlag = false;
            _customerExpectationFlag = false;
            _activeCustomer = await _customerFetcher.GetNextCustomer();
            
            await _activeCustomer.ExecuteEntering(token);
            _gameplayService.UpdateCustomerInfo(1f);
            
            await UniTask.WaitUntil(() => ActiveFlag, cancellationToken: token);
            await UniTask.WaitForSeconds(GameplayConfig.Instance.BubblePassDelay, cancellationToken: token);
            _dialogueBubble.Activate(_activeCustomer.Order).Forget();
            await UniTask.WaitUntil(() => _dialogueBubble.BubbleOpenedFlag is false, cancellationToken: token);

            
            if (_activeCustomer.Order.NeedWaitTime)
            {
                _customerExpectationTimer.RunWithDuration(_activeCustomer.Order.TimeToReady);
                _customerExpectationFlag = true;   
            }

            _customerLoyalty = 1f;
            var giveAwayPotionFlag = false;
            var subToPotionGive = _activeCustomer.GetPotionEvent.First().Subscribe(_ => giveAwayPotionFlag = true);
            
            await UniTask.WaitUntil(() => (_customerExpectationFlag && (_customerLoyalty <= 0.01f)) || giveAwayPotionFlag, cancellationToken: token);
            subToPotionGive?.Dispose();

            var isCorrectResult = giveAwayPotionFlag && _activeCustomer.Order.RequestedItemID == _potionDummy.ID; 
            
            if (giveAwayPotionFlag)
            {
                _potionDummy.gameObject.SetActive(false);
                _gameplayService.Data.SetCraftedPotion(null);
            }

            if (isCorrectResult)
            {
                _gameplayService.Data.AddCustomerCoinToBalance(_activeCustomer.Order.Reward);
            }
            
            if (giveAwayPotionFlag is false)
            {
                if (_gameplayService.CurrentGameScene != GameScene.Hall)
                {
                    _gameplayService.UI.HudUI.ShowClientLeaveIndicator().Forget();
                }
            }
            
            await UniTask.WaitForSeconds(GameplayConfig.Instance.BubblePassDelay, cancellationToken: token);
            _dialogueBubble.Activate(showButtons: false);
            await _dialogueBubble.FillTextFld(isCorrectResult ? _activeCustomer.GoodFarewellWord.Mess : _activeCustomer.BadFarewellWord.Mess, token)
                .ContinueWith(() => _dialogueBubble.Deactivate());
            
            await UniTask.WaitUntil(() => _dialogueBubble.BubbleOpenedFlag is false, cancellationToken: token);
            
            ExecuteLeaveCustomer(token).Forget();
        }

        [Button(nameof(ExecuteLeaveCustomer))]
        private async UniTask ExecuteLeaveCustomer(CancellationToken token)
        {
            _customerExpectationFlag = false;
            await _activeCustomer.ExecuteLeaving(token);
            
            _activeCustomer = null;
            _freeHallFlag = true;
            
            await UniTask.WaitForSeconds(GameplayConfig.Instance.GetDelayBetweenCustomers(_gameplayService.GameTimer.TimeRatio), cancellationToken: stateProcess.Token);
            LaunchCustomer(stateProcess.Token).Forget();
        }
    }
} 