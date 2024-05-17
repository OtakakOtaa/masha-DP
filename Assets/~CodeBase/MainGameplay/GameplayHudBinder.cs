using System;
using _CodeBase.Infrastructure;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.MainGameplay
{
    public sealed class GameplayHudBinder : MonoBehaviour
    {
        [SerializeField] private Button _locationBtn1;
        [SerializeField] private Image _locationBtn1Image;
        [SerializeField] private Button _locationBtn2;
        [SerializeField] private Image _locationBtn2Image;
        [SerializeField] private Button _menuBtn;

        [SerializeField] private TMP_Text _timeFld;
        [SerializeField] private TMP_Text _customerIndicatorFld;
        [SerializeField] private TMP_Text _coinsFld;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        
        [SerializeField] private Sprite _normalCustomerEmj;
        [SerializeField] private Sprite _angryCustomerEmj;
        
        [SerializeField] private Sprite _hallIcon;
        [SerializeField] private Sprite _gardenIcon;
        [SerializeField] private Sprite _potionIcon;


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

            _gameplayService.StatsChangedEvent
                .Subscribe(_ => UpdateCoins(_gameplayService.Data.Coins))
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
                .Subscribe(_ => UpdateTime(_gameplayService.GameTimer.Value))
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
        public void UpdateTime(TimeSpan span)
            => _timeFld.text = span.ToString(@"\mm\:ss");

        [Button]
        public void UpdateCustomerIndicator(float customerLoyalty)
        {
            _customerIndicatorFld.text = $"{(int)(customerLoyalty * 100)}%";
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