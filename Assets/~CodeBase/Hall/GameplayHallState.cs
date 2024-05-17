using System;
using System.Threading;
using _CodeBase.Customers;
using _CodeBase.Customers._Data;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        [Space] [SerializeField] private Transform _rackPoint;
        [SerializeField] private Transform _entryPoint;
        [SerializeField] private float _customerEntryAnimDuration;
        [SerializeField] private float _customerLeaveAnimDuration;
        [SerializeField] private float _entryAnimYFrequency = 1;
        [SerializeField] private float _entryAnimYAmplitude = 1;
        [SerializeField] private float _spanBetweenCustomers = 1;
        [SerializeField] private float _bubblePassTime = 1;
        

        private Customer _activeCustomer;
        private bool _freeHallFlag;
        private bool _customerExpectationFlag;
        private float _customerReceivedTimePoint;
        private float _customerLeaveTimePoint;

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
            if (_customerExpectationFlag)
            {
                var customerLoyalty = (_customerLeaveTimePoint - Time.time) /
                                      (_customerLeaveTimePoint - _customerReceivedTimePoint);
                
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
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Button(nameof(LaunchCustomer))]
        private async UniTask LaunchCustomer(CancellationToken token)
        {
            _freeHallFlag = false;
            _customerExpectationFlag = false;
            
            _activeCustomer = _customerFetcher.GetNextCustomer();

            await ExecuteCustomerMovementAnim(_entryPoint.position, _rackPoint.position, _customerEntryAnimDuration,
                token);

            await UniTask.WaitUntil(() => ActiveFlag, cancellationToken: token);
            await UniTask.WaitForSeconds(_bubblePassTime, cancellationToken: token);
            _dialogueBubble.Activate(_activeCustomer.Order).Forget();
            await UniTask.WaitUntil(() => _dialogueBubble.BubbleOpenedFlag is false, cancellationToken: token);

            _customerReceivedTimePoint = Time.time;
            _customerLeaveTimePoint = Time.time + _activeCustomer.Order.TimeToReady - (float)_gameplayService.GameTimer.Value.TotalSeconds * 0.001f;
            
            _customerExpectationFlag = true;
        }

        [Button(nameof(ExecuteLeaveCustomer))]
        private async UniTask ExecuteLeaveCustomer(CancellationToken token)
        {
            await ExecuteCustomerMovementAnim(_rackPoint.position, _entryPoint.position, _customerLeaveAnimDuration,
                token);

            _activeCustomer = null;
            _freeHallFlag = true;
        }

        private async UniTask ExecuteCustomerMovementAnim(Vector3 start, Vector3 end, float duration,
            CancellationToken token)
        {
            _activeCustomer.transform.position = start;

            var anim = DOTween.Sequence()
                .Append(DOTween.To
                (
                    setter: value =>
                    {
                        var pos = Vector3.Lerp(start, end, value);
                        var yPos = (float)Math.Sin(Math.PI * _entryAnimYFrequency * duration / 4 * 2 * (value - 0.5)) * _entryAnimYAmplitude;
                        _activeCustomer.transform.position = new Vector3(pos.x, pos.y + yPos, pos.z);
                    },
                    duration: duration,
                    startValue: 0,
                    endValue: 1
                ).SetEase(Ease.Flash))
                .Join(_activeCustomer.transform.DOPunchRotation
                (
                    punch: Vector3.up,
                    duration: duration
                )).Play();

            Action customerAnim = () => anim.Kill();
            StateDisposeHandler += customerAnim;
            await UniTask.WaitForSeconds(duration, cancellationToken: token);
            StateDisposeHandler -= customerAnim;
        }
    }
} 