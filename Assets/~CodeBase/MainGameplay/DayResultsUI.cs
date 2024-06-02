using System;
using _CodeBase.Infrastructure.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _CodeBase.MainGameplay
{
    public sealed class DayResultsUI : MonoBehaviour
    {
        [SerializeField] private Image _mainIcon;
        [SerializeField] private TMP_Text _earnedCoinTMP;
        [SerializeField] private Button _continueBtn;
        [SerializeField] private Sprite[] _mainIconsSprites;
        [SerializeField] private AppearsAnimation _appearsAnimation;
        
        
        public IObservable<Unit> ContinueClickedEvent => _continueBtn.OnClickAsObservable();
        public bool ContinueEventFlag { get; private set; }

        
        private void Awake()
        {
            _continueBtn.OnClickAsObservable().Subscribe(_ => ContinueEventFlag = true).AddTo(destroyCancellationToken);
        }

        public void Init(int earnedCoinsAmount)
        {
            ContinueEventFlag = false;
            var randomizedSprite = _mainIconsSprites[Random.Range(0, maxExclusive: _mainIconsSprites.Length)];
            
            _mainIcon.sprite = randomizedSprite;
            _mainIcon.SetNativeSize();
            _earnedCoinTMP.text = earnedCoinsAmount.ToString();
        }

        public void Show()
        {
            _appearsAnimation.Play();
        }
    }
}