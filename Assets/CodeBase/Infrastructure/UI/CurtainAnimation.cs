using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CodeBase.Infrastructure.UI
{
    public sealed class CurtainAnimation : MonoBehaviour, IAnimation
    {
        [SerializeField] private AppearsAnimation _appears;
        [SerializeField] private DisappearanceAnimation _disappearance;
        [SerializeField] private float _duration = 5;
        [SerializeField] private float _defaultAppearsDuration = 0.8f;
        [SerializeField] private float _defaultDisappearanceDuration = 0.6f;
        
        
        public Tween Tween { get; }
        
        public float DefaultDisappearanceDuration => _defaultDisappearanceDuration;
        public float DefaultAppearsDuration => _defaultAppearsDuration;

        
        public async void Play(float? duration = null)
        {
            duration ??= _duration;
            
            _appears.Play(duration / 2f);
            await UniTask.WaitForSeconds((float)(duration / 2f));
            await UniTask.WaitForSeconds((float)(duration / 3f));
            _disappearance.Play(duration / 5f);
            await UniTask.WaitForSeconds((float)(duration / 5f));
        }

        public async UniTask PlayAppears(float? duration = null)
        {
            duration ??= DefaultAppearsDuration;
            _disappearance.Tween?.Kill();
            _appears.Play(duration);
            await UniTask.WaitUntil(() => _appears.Tween.IsComplete() || _appears.Tween.active is false);
        }

        public async UniTask PlayDisappearance(float? duration = null)
        {
            duration ??= DefaultDisappearanceDuration;
            _appears.Tween?.Kill();
            _disappearance.Play(duration);
            await UniTask.WaitUntil(() => _disappearance.Tween.IsComplete() || _disappearance.Tween.active is false);
        }
    }
}