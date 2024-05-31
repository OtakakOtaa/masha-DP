using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Infrastructure.UI
{
    public sealed class DisappearanceAnimation : MonoBehaviour, IAnimation
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
                _canvasGroup.alpha = 1;
                _tween = _canvasGroup.DOFade(0f, _duration)
                    .SetEase(_ease)
                    .OnComplete(() => gameObject.SetActive(false));
                
                return;
            }

            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
            
            _tween = _image.DOFade(0f, _duration)
                .SetEase(_ease)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}