using System;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Infrastructure.UI
{
    [RequireComponent(typeof(Button))]
    public class PulledUIItem : InteractiveObject
    {
        [SerializeField] private float _releaseDistance = 50f;
        [SerializeField] private float _returnDuration = 0.3f;
        [SerializeField] private Vector2 _endPositionOffset;
        
        [SerializeField] private Vector2 _direction;
        [SerializeField] private Ease _anim;
        [SerializeField] private float _sensitivity = 1;

        private RectTransform _rectTransform;
        private Vector2 _initialPosition;
        private bool _isExecuted = false;
        private Tweener _executeAnim;
        private Tweener _backAnim;

        public ReactiveCommand OnExecuted { get; private set; } = new();
        
        
        protected override void OnAwake()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.anchoredPosition;
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }


        public void SetToDefault()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            
            _rectTransform.anchoredPosition = _initialPosition;
        }

        public void SetToDefaultWithAnim()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            _backAnim?.Kill();
            
            var startPos = _initialPosition + _endPositionOffset;
            _backAnim = DOTween.To(setter: v => _rectTransform.anchoredPosition = Vector2.Lerp(startPos, _initialPosition, v), startValue: 0f, endValue: 1f, _returnDuration).SetEase(_anim);
        }

        
        public void SetCurrentPositionAsInit()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            _initialPosition = _rectTransform.anchoredPosition;
        }
        
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (_isExecuted || enabled is false) return;

            var cam = GameplayService.Instance.Camera;

            var mousePos = (Vector2)cam.ScreenToViewportPoint(InputManager.Instance.ScreenMousePos) - (Vector2)cam.WorldToViewportPoint(_rectTransform.transform.position);;
            var delta = mousePos * Time.deltaTime * _sensitivity;
            
            
            
            _rectTransform.anchoredPosition += delta * new Vector2(Math.Abs(_direction.x), Math.Abs(_direction.y)).normalized;

            var clampDelta = (_rectTransform.anchoredPosition - _initialPosition) * _direction.normalized;
            if (clampDelta.x < 0f || clampDelta.y < 0f)
            {
                _rectTransform.anchoredPosition = _initialPosition;
            }
            
            if ((_rectTransform.anchoredPosition - _initialPosition).magnitude + _releaseDistance / 5f < _releaseDistance) return;

            PlayExecuteAnimation();
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            if (_isExecuted) return;

            _backAnim?.Kill();
            
            var startPos = _rectTransform.anchoredPosition;
            _backAnim = DOTween.To(setter: v => _rectTransform.anchoredPosition = Vector2.Lerp(startPos, _initialPosition, v), startValue: 0f, endValue: 1f, _returnDuration).SetEase(_anim);
            
            _rectTransform.anchoredPosition = _initialPosition;
        }

        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            if (_isExecuted || enabled is false || _backAnim?.IsComplete() is false) return;

            _backAnim?.Kill();
            _isExecuted = false;
            _rectTransform.anchoredPosition = _initialPosition;
        }

        public void PlayExecuteAnimation(bool isNeedExecuted = true)
        {
            _isExecuted = true;

            _executeAnim?.Kill();
            var pos = _rectTransform.anchoredPosition;

            _executeAnim = DOTween.To(setter: v => _rectTransform.anchoredPosition = Vector2.Lerp(_initialPosition + _endPositionOffset, pos, v), startValue: 0f, endValue: 1f, _returnDuration)
                .SetEase(_anim)
                .SetInverted(true)
                .OnComplete(() =>
                {
                    _isExecuted = false;
                    if (isNeedExecuted) OnExecuted.Execute();
                });
        }
    }
}