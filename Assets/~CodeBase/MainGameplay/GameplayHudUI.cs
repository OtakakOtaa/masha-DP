using System;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.UI;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _CodeBase.MainGameplay
{
    public sealed class GameplayHudUI : MonoBehaviour
    {
        [SerializeField] private Button _locationBtn1;
        [SerializeField] private Image _locationBtn1Image;
        [SerializeField] private Button _locationBtn2;
        [SerializeField] private Image _locationBtn2Image;
        [SerializeField] private Button _menuBtn;

        [SerializeField] private TMP_Text _timeFld;
        [SerializeField] private Image _timeSignImage;
        
        [SerializeField] private TMP_Text _customerIndicatorFld;
        [SerializeField] private Image _customerIndicatorImage;
        [SerializeField] private TMP_Text _coinsFld;

        [SerializeField] private AppearsAnimation _clientLeaveNotifyAppearAnim;
        [SerializeField] private DisappearanceAnimation _clientLeaveNotifyDisappearAnim;

        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

        [SerializeField] private Sprite _goodCustomerEmj;
        [SerializeField] private float _goodCustomerBottomThreshold;
        
        [SerializeField] private Sprite _normalCustomerEmj;
        [SerializeField] private float _normalCustomerBottomThreshold;
        
        [SerializeField] private Sprite _angryCustomerEmj;
        
        [SerializeField] private Sprite _hallIcon;
        [SerializeField] private Sprite _gardenIcon;
        [SerializeField] private Sprite _potionIcon;
        
        [SerializeField] private float _startDayHTime = 12;
        [SerializeField] private float _endDatHTime = 18;
        
        
        private readonly CompositeDisposable _compositeDisposable = new();
        private GameplayService _gameplayService;

        
        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }

        public void Bind(GameplayService gameplayService)
        {
            _locationBtn1Image.sprite = _potionIcon;
            _locationBtn2Image.sprite = _gardenIcon;
            
            _gameplayService = gameplayService;

            _gameplayService.Data.CoinsBalanceChangedEvent
                .Subscribe(_ => UpdateCoins(_gameplayService.Data.GlobalCoins))
                .AddTo(_compositeDisposable);
            
            _locationBtn1.OnClickAsObservable()
                .Subscribe(_ => HandleLocBtn1Press())
                .AddTo(_compositeDisposable);
            
            _locationBtn2.OnClickAsObservable()
                .Subscribe(_ => HandleLocBtn2Press())
                .AddTo(_compositeDisposable);

            _menuBtn.OnClickAsObservable()
                .Subscribe(_ => GoToMainMenu())
                .AddTo(_compositeDisposable);

            Observable.EveryUpdate()
                .Subscribe(_ => UpdateTime(_gameplayService.GameTimer.TimeRatio))
                .AddTo(_compositeDisposable);
            
            
            UpdateCoins(amount: 0);
            UpdateCustomerIndicator(customerLoyalty: 1);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

        [Button]
        public void GoToGarden()
        {
            _locationBtn1Image.sprite = _hallIcon;
            _locationBtn2Image.sprite = _potionIcon;
            _gameplayService.GoToGardenLac().Forget();
        }

        [Button]
        public void GoToLaboratory()
        {
            _locationBtn1Image.sprite = _gardenIcon;
            _locationBtn2Image.sprite = _hallIcon;
            _gameplayService.GoToPotionLac().Forget();
        }

        [Button]
        public void GoToHall()
        {
            _locationBtn1Image.sprite = _potionIcon;
            _locationBtn2Image.sprite = _gardenIcon;
            _gameplayService.GoToHallLac().Forget();
        }
        
        [Button]
        public void GoToMainMenu()
            => _gameplayService.GoToMainMenu().Forget();

        [Button]
        public void UpdateCoins(int amount)
            => _coinsFld.text = amount.ToString();

        [Button]
        public void UpdateTime(float progress)
        {
            _timeSignImage.fillAmount = progress;
            var h = Mathf.Lerp(_startDayHTime, _endDatHTime, progress);
            
            _timeFld.text = TimeSpan.FromHours(h).ToString(@"\hh\:mm");
        }

        [Button]
        public void UpdateCustomerIndicator(float customerLoyalty)
        {
            var loyaltyPer = (int)(customerLoyalty * 100f);
            
            _customerIndicatorFld.text = $"{loyaltyPer}%";

            if (_goodCustomerBottomThreshold < loyaltyPer)
            {
                _customerIndicatorImage.sprite = _goodCustomerEmj;
                return;
            }

            if (_normalCustomerBottomThreshold < loyaltyPer)
            {
                _customerIndicatorImage.sprite = _normalCustomerEmj;
                return;
            }
            
            _customerIndicatorImage.sprite = _angryCustomerEmj;
        }

        public async UniTaskVoid ShowClientLeaveIndicator()
        {
            _clientLeaveNotifyAppearAnim.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: destroyCancellationToken);
            _clientLeaveNotifyDisappearAnim.Play();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        
        private void HandleLocBtn1Press()
        {
            switch (_gameplayService.CurrentGameScene)
            {
                case GameScene.Hall:
                    GoToLaboratory();
                    break;
                case GameScene.Garden:
                    GoToHall();
                    break;
                case GameScene.Laboratory:
                    GoToGarden();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleLocBtn2Press()
        {
            switch (_gameplayService.CurrentGameScene)
            {
                case GameScene.Hall:
                    GoToGarden();
                    break;
                case GameScene.Garden:
                    GoToLaboratory();
                    break;
                case GameScene.Laboratory:
                    GoToHall();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}