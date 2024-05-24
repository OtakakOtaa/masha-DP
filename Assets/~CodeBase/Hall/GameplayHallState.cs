using System.Threading;
using _CodeBase.Customers;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.Hall
{
    public sealed class GameplayHallState : GameplaySceneState
    {
        [Inject] private GameplayService _gameplayService;
        [SerializeField] private CustomerFetcher _customerFetcher;
        [SerializeField] private DialogueBubble _dialogueBubble;

        [SerializeField] private float _spanBetweenCustomers = 1;
        [SerializeField] private float _bubblePassTime = 1;
        

        private Customer _activeCustomer;
        private bool _freeHallFlag;
        private bool _customerExpectationFlag;
        private float _customerReceivedTimePoint;
        private float _customerLeaveTimePoint;
        private bool _isNeededExpectation;

        private void Awake()
        {
            GameService.GameUpdate.Subscribe(_ => OnUpdate()).AddTo(destroyCancellationToken);
        }

        protected override void OnFirstEnter()
        {
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
            if (!_customerExpectationFlag) return;

            
            var customerLoyalty = _isNeededExpectation ? (_customerLeaveTimePoint - Time.time) / (_customerLeaveTimePoint - _customerReceivedTimePoint) : 100f;
            
            if (customerLoyalty <= 0.01f)
            {
                customerLoyalty = 0f;
                _customerExpectationFlag = false;
                ExecuteLeaveCustomer(stateProcess.Token).ContinueWith(async () =>
                {
                    await UniTask.WaitForSeconds(_spanBetweenCustomers, cancellationToken: stateProcess.Token);
                    LaunchCustomer(stateProcess.Token).Forget();
                }).Forget();
            }
                
            _gameplayService.UpdateCustomerInfo(customerLoyalty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Button(nameof(LaunchCustomer))]
        private async UniTask LaunchCustomer(CancellationToken token)
        {
            _freeHallFlag = false;
            _customerExpectationFlag = false;
            
            _activeCustomer = await _customerFetcher.GetNextCustomer();

            await _activeCustomer.ExecuteEntering(token);

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
        }

        [Button(nameof(ExecuteLeaveCustomer))]
        private async UniTask ExecuteLeaveCustomer(CancellationToken token)
        {
            await _activeCustomer.ExecuteLeaving(token);

            _activeCustomer = null;
            _freeHallFlag = true;
        }
    }
} 