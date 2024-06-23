using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _CodeBase.Infrastructure.UI
{
    public sealed class RotationAnimation : MonoBehaviour
    {
        [SerializeField] private float _valueFactor = 1;
        [SerializeField] private Vector3 _axis = Vector3.forward;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        [SerializeField] private float _speedFactor = 1f;
        

        
        private Quaternion _originRotation;
        private float _delta;


        private void Awake()
        {
            _originRotation = transform.localRotation;
            GameService.GameUpdate.Subscribe(_ => OnAnimationUpdate()).AddTo(destroyCancellationToken);
        }

        private void OnAnimationUpdate()
        {
            _delta += Time.deltaTime * _speedFactor;
            if (_delta >= 1f) _delta = 0f;
            
            var delta = _curve.Evaluate(_delta) * _valueFactor;
            transform.localRotation = _originRotation * Quaternion.Euler(_axis * delta);
        }
    }
}