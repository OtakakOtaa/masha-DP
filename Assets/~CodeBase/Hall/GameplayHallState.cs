using System.Threading;
using _CodeBase.Customers;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.Hall
{
    public sealed class GameplayHallState : GameplaySceneState
    {
        [SerializeField] private PotionDummy _potionDummy;
        [SerializeField] private CustomerFetcher _customerFetcher;
        [SerializeField] private DialogueBubble _dialogueBubble;

        [SerializeField] private float _spanBetweenCustomers = 1;
        [SerializeField] private float _bubblePassTime = 1;


        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        
        
        private Customer _activeCustomer;
        private bool _freeHallFlag;
        private bool _customerExpectationFlag;
        private float _customerReceivedTimePoint;
        private float _customerLeaveTimePoint;
        private bool _isNeededExpectation;
        private float _customerLoyalty;
        private bool _isPotionDummyActive;

        
        private void Awake()
        {
            GameService.GameUpdate.Subscribe(_ => OnUpdate()).AddTo(destroyCancellationToken);
        }

        protected override void OnFirstEnter()
        {
            _potionDummy.gameObject.SetActive(false);
            _isPotionDummyActive = false;
            
            _customerFetcher.Init();
            LaunchCustomer(stateProcess.Token).Forget();
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        private void OnUpdate()
        {
            if (!_isPotionDummyActive && !string.IsNullOrEmpty(_gameplayService.Data.CraftedPotion))
            {
                _isPotionDummyActive = true;
                _potionDummy.gameObject.SetActive(true);
                _potionDummy.Init(_gameConfigProvider.GetByID<PotionConfig>(_gameplayService.Data.CraftedPotion));
            }
            
            if (!_customerExpectationFlag) return;
            
            _customerLoyalty = _isNeededExpectation ? (_customerLeaveTimePoint - Time.time) / (_customerLeaveTimePoint - _customerReceivedTimePoint) : 100f;
            if (_customerLoyalty <= 0.01f) _customerLoyalty = 0f;
            _gameplayService.UpdateCustomerInfo(_customerLoyalty);
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
            await UniTask.WaitForSeconds(_bubblePassTime, cancellationToken: token);
            _dialogueBubble.Activate(_activeCustomer.Order).Forget();
            await UniTask.WaitUntil(() => _dialogueBubble.BubbleOpenedFlag is false, cancellationToken: token);

            
            _isNeededExpectation = _activeCustomer.Order.NeedWaitTime;
            
            if (_activeCustomer.Order.NeedWaitTime)
            {
                _customerReceivedTimePoint = Time.time;
                _customerLeaveTimePoint = Time.time + _activeCustomer.Order.TimeToReady - (float)_gameplayService.GameTimer.Value.TotalSeconds * 0.001f;
                _customerExpectationFlag = true;   
            }

            var givePotionFlag = false;
            var subToPotionGive = _activeCustomer.GetPotionEvent.First().Subscribe(_ => givePotionFlag = true);

            await UniTask.WaitUntil(() => (_customerExpectationFlag && (_customerLoyalty <= 0.01f)) || givePotionFlag, cancellationToken: token);
            subToPotionGive?.Dispose();

            if (givePotionFlag)
            {
                _isPotionDummyActive = false;
                _potionDummy.gameObject.SetActive(false);
                _gameplayService.Data.SetCraftedPotion(null);
            }

            if (givePotionFlag && _activeCustomer.Order.RequestedItemID == _potionDummy.ID)
            {
                _gameplayService.Data.ChangeCoinBalance(_gameplayService.Data.Coins + _activeCustomer.Order.Reward);
            }

            
            ExecuteLeaveCustomer(token).Forget();
        }

        [Button(nameof(ExecuteLeaveCustomer))]
        private async UniTask ExecuteLeaveCustomer(CancellationToken token)
        {
            _customerExpectationFlag = false;
            await _activeCustomer.ExecuteLeaving(token);
            
            _activeCustomer = null;
            _freeHallFlag = true;
            
            await UniTask.WaitForSeconds(_spanBetweenCustomers, cancellationToken: stateProcess.Token);
            LaunchCustomer(stateProcess.Token).Forget();
        }
    }
} 