using System;
using _CodeBase.Infrastructure.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _CodeBase.MainGameplay
{
    public sealed class StartDayUI : MonoBehaviour
    {
        [Serializable] private sealed class VisualVariant
        {
            [field: PreviewField]
            [field: SerializeField] public Sprite Background { get; private set; }
            [field: SerializeField] public Sprite Clothespin { get; private set; }
            
            [field: SerializeField] public Color MainColor { get; private set; }
            [field: SerializeField] public Color SammyColor { get; private set; }
        }

        
        [SerializeField] private VisualVariant[] _visualVariants;
        [SerializeField] private Image[] _mainImages;
        [SerializeField] private Image[] _sammyImages;
        [SerializeField] private Graphic[] _sammyGraphics;
        
        [SerializeField] private Image _bg;
        [SerializeField] private Image _clothespin;
        [SerializeField] private TMP_Text _dayTMP;
        [SerializeField] private AppearsAnimation _appearsAnimation;
        [SerializeField] private DisappearanceAnimation _disappearanceAnimation;
        
            
        public void InitAndShow(int day, bool needAnimation = true)
        {
            var currentVariant = _visualVariants[Random.Range(0, _visualVariants.Length)];
            _bg.sprite = currentVariant.Background;
            _bg.color = Color.white;
            
            foreach (var image in _mainImages)
            {
                image.color = currentVariant.MainColor;
            }

            foreach (var image in _sammyImages)
            {
                image.color = currentVariant.SammyColor;
            }

            _clothespin.sprite = currentVariant.Clothespin;
            _clothespin.color = Color.white;
            _dayTMP.text = day.ToString();
            
            foreach (var graphic in _sammyGraphics)
            {
                graphic.color = currentVariant.SammyColor;
            }

            if (needAnimation) _appearsAnimation.Play();
            else gameObject.SetActive(true);
        }

        public void Close()
        {
            _disappearanceAnimation.Play();
        }
    }
}