using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _CodeBase.Infrastructure.UI
{
    public sealed class CurtainAnimation : MonoBehaviour, IAnimation
    {
        [SerializeField] private AppearsAnimation _appears;
        [SerializeField] private DisappearanceAnimation _disappearance;
        [SerializeField] private float _duration = 5;
        
        
        public Tween Tween { get; }

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
            _disappearance.Tween?.Kill();
            _appears.Play(duration);
            await UniTask.WaitUntil(() => _appears.Tween.IsComplete() || _appears.Tween.active is false);
        }

        public async UniTask PlayDisappearance(float? duration = null)
        {
            _appears.Tween?.Kill();
            _disappearance.Play(duration);
            await UniTask.WaitUntil(() => _disappearance.Tween.IsComplete() || _disappearance.Tween.active is false);
        }
    }
}