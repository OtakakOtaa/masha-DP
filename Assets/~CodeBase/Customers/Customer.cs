using System.Threading;
using _CodeBase.Customers._Data;
using _CodeBase.Hall;
using _CodeBase.Infrastructure;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _CodeBase.Customers
{
    public sealed class Customer : ObjectKeeper
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _startAnimPos;
        [SerializeField] private Transform _endAnimPos;
        [SerializeField] private AnimationCurve _animEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _animationDuration;
        [SerializeField] private GifAnimation _gifAnimationPlayer;
        
        
        private Tween _movementAnimTween;


        public ReactiveCommand<PotionConfig> GetPotionEvent { get; private set; } = new();
        public ReactiveCommand CustomerEnterEnded { get; private set; } = new();
        public Order Order { get; private set; }
        public CustomerInfo CustomerInfo { get; private set; }
        public CustomerFarewellWord GoodFarewellWord { get; private set; }
        public CustomerFarewellWord BadFarewellWord { get; private set; }
        public CustomerVisual CustomerVisual { get; private set; }
        
        
        
        
        protected override void OnAwake() { }

        public Customer Init(CustomerVisual visual, Order order, CustomerInfo data, CustomerFarewellWord goodFarewellWord, CustomerFarewellWord badFarewellWord)
        {
            CustomerVisual = visual;
            Order = order;
            CustomerInfo = data;
            GoodFarewellWord = goodFarewellWord;
            BadFarewellWord = badFarewellWord;
            
            _spriteRenderer.sprite = visual.Sprite;
            _spriteRenderer.color = visual.MainColor;
            
            return this;
        }


        public async UniTask ExecuteEntering(CancellationToken token)
        {
            InvokeMovementAnimation(rewindFlag: false);
            
            var awaiter = new UniTaskCompletionSource();
            _movementAnimTween.OnComplete(() => awaiter.TrySetResult());

            await awaiter.Task.AttachExternalCancellation(token);
        }

        public async UniTask ExecuteLeaving(CancellationToken token)
        {
            InvokeMovementAnimation(rewindFlag: true);
            
            var awaiter = new UniTaskCompletionSource();
            _movementAnimTween.OnComplete(() => awaiter.TrySetResult());

            await awaiter.Task.AttachExternalCancellation(token);
        }

        public async UniTask PlayHappyTalk()
        {
            await _gifAnimationPlayer.Play(gifInfo: CustomerVisual.HappyTalkAnim, loopFlag: true);
        }
        
        public async UniTask PlaySadTalk()
        {
            await _gifAnimationPlayer.Play(gifInfo: CustomerVisual.SadTalkAnim, loopFlag: true);
        }

        public void StopTalk()
        {
            _gifAnimationPlayer.Kill();
            _spriteRenderer.sprite = CustomerVisual.Sprite;
        }
        
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (_inputManager.GameplayCursor.HandleItem is PotionDummy potionDummy)
            {
                GetPotionEvent?.Execute(potionDummy.Config);
            }
        }

        
        private void InvokeMovementAnimation(bool rewindFlag)
        {
            _movementAnimTween?.Kill();
            
            _movementAnimTween = DOTween.To(f => transform.position = Vector3.LerpUnclamped(_startAnimPos.position, _endAnimPos.position, f), startValue: 0f, endValue: 1f, duration: _animationDuration)
                .SetEase(_animEase)
                .SetInverted(rewindFlag);
        }
    }
}