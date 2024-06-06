using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using DG.Tweening;
using UnityEngine;

namespace _CodeBase.Infrastructure
{
    public sealed class IntractableAnimation : InteractiveObject
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _strength = 10f;
        [SerializeField] private int _vibrationsCounter = 20;
        
        [Range(0f, 1f)] 
        [SerializeField] private float _rotationFactor = 1;

        [Range(0f, 1f)] 
        [SerializeField] private float _scaleFactor = 0.1f;

        [Range(0f, 1f)] 
        [SerializeField] private float _positionFactor = 0f;
        
        
        private Sequence _tween;
        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.DoubleClick);
        }

        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            _tween?.Complete();

            _tween = DOTween.Sequence()
                .Append(transform.DOShakeRotation(duration: _duration, strength: _strength * _rotationFactor, vibrato: _vibrationsCounter))
                .Join(transform.DOShakePosition(duration: _duration, strength: _strength * _positionFactor, _vibrationsCounter))
                .Join(transform.DOShakeScale(duration: _duration, strength: _strength * _scaleFactor, _vibrationsCounter));
        }
    }
}