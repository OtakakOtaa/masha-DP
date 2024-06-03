using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _CodeBase.Infrastructure
{
    public sealed class DumpAnimation : MonoBehaviour
    {
        [SerializeField] private float _valueFactor = 1;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        [SerializeField] private Vector3 _axis = Vector3.up;
        [SerializeField] private float _speedFactor = 1f;

        private Vector3 _originPos;
        private float _delta;
        
        private void Awake()
        {
            _originPos = transform.localPosition;
            GameService.GameUpdate.Subscribe(_ => OnAnimationUpdate()).AddTo(destroyCancellationToken);
        }

        private void OnAnimationUpdate()
        {
            _delta += Time.deltaTime * _speedFactor;
            if (_delta >= 1f) _delta = 0f;
            
            var delta = _curve.Evaluate(_delta) * _valueFactor;
            transform.localPosition = _originPos + (_axis * delta);
        }
    }
}