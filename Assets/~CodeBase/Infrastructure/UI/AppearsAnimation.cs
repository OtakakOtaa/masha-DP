using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Infrastructure.UI
{
    public sealed class AppearsAnimation : MonoBehaviour, IAnimation
    {
        [SerializeField] private Image _image;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

        private Tween _tween;
        
        
        public void Play()
        {
            _tween?.Kill();
            gameObject.SetActive(true);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _tween = _canvasGroup.DOFade(1f, _duration).SetEase(_ease);
                return;
            }

            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
            _tween = _image.DOFade(1f, _duration).SetEase(_ease);
        }
    }
}