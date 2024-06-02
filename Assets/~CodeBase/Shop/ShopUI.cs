using System;
using System.Collections.Generic;
using _CodeBase.Customers;
using _CodeBase.DATA;
using _CodeBase.MainGameplay;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace _CodeBase.Shop
{
    public sealed class ShopUI : MonoBehaviour
    {
        [Serializable] public sealed class FilterBtnBinder
        {
            [SerializeField] public Button button;
            [SerializeField] public UniqItemsType uniqItemsType;
        }
        
        
        [SerializeField] private TMP_Text _coins;
        [FormerlySerializedAs("_mainMenuBtn")] [SerializeField] private Button _returnBtn;
        [FormerlySerializedAs("_mainContinueBtn")] [SerializeField] private Button _continueBtn;
        [SerializeField] private Sprite _selectedFilterBtnSprite;

        
        [DictionaryDrawerSettings]
        [SerializeField] private FilterBtnBinder[] _filterBtns;
        
        
        [SerializeField] private ShopUIItem _shopUIItemPrefab;
        [SerializeField] private Transform _container;


        [Inject] private ShopConfigurationProvider _shopConfiguration;
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private GameplayService _gameplayService;

        private CompositeDisposable _itemsHooksHandler = new();
        private readonly List<ShopUIItem> _shopUIItemInstances = new();
        private readonly Dictionary<string, int> _shopUIItemInstancesIdsMap = new();
        
        private readonly Dictionary<UniqItemsType, HashSet<int>> _typesDefinitionBrowser = new();
        private UniqItemsType _currentFilter = UniqItemsType.None;


        public readonly ReactiveCommand<string> BuyBtnPressEvent = new();
        public bool ReturnToSignalFlag { get; private set; }
        public bool ContinueSignalFlag { get; private set; }
        
        
        
        private void Awake()
        {
            _itemsHooksHandler = new CompositeDisposable();
            
            foreach (var filterBtn in _filterBtns)
            {
                var filterBtnMock = filterBtn;
                
                filterBtn.button.OnClickAsObservable()
                    .Where(_ => _currentFilter != filterBtnMock.uniqItemsType)
                    .Subscribe(_ => ShowOnlyType(filterBtn.uniqItemsType))
                    .AddTo(destroyCancellationToken);
            }

            _gameplayService.Data.CoinsBalanceChangedEvent.Subscribe(c => _coins.text = c.ToString()).AddTo(destroyCancellationToken);
            
            _gameplayService.Data.DataAddedEvent
                .Where(id => _shopUIItemInstancesIdsMap.ContainsKey(id))
                .Subscribe(HandleItemRemove)
                .AddTo(destroyCancellationToken);
            
            _continueBtn.OnClickAsObservable().Subscribe(_ => ContinueSignalFlag = true).AddTo(destroyCancellationToken);
            _returnBtn.OnClickAsObservable().Subscribe(_ => ReturnToSignalFlag = true).AddTo(destroyCancellationToken);

            gameObject.OnDestroyAsObservable().First().Subscribe(_ => _itemsHooksHandler?.Dispose());
        }


        public void Init(string[] items)
        {
            _itemsHooksHandler?.Dispose();
            _itemsHooksHandler = new CompositeDisposable();
            ReturnToSignalFlag = false;
            ContinueSignalFlag = false;
            _typesDefinitionBrowser.Clear();
            _currentFilter = UniqItemsType.None;
            
            UpdateScrollContentSize(items.Length);
            
            for (var i = 0; i < items.Length; i++)
            {
                var conf = _gameConfigProvider.GetByID<IUniq>(items[i]);
                
                var type = _gameConfigProvider.TryDefineTypeByID(items[i]);
                if (type == UniqItemsType.None) throw new Exception(nameof(ShopUI));

                if (!_typesDefinitionBrowser.ContainsKey(type)) _typesDefinitionBrowser[type] = new HashSet<int>();

                var shopConfig = _shopConfiguration.GetDataByID(conf.ID);
                if (shopConfig == null) throw new Exception($"{conf.ID} shopWindow: can't found item");
                
                _shopUIItemInstances[i].gameObject.SetActive(true);
                _shopUIItemInstances[i].Init(conf.Sprite, shopConfig.Name, shopConfig.Cost, shopConfig.Rect);
                
                _shopUIItemInstancesIdsMap[shopConfig.ID] = i;
                
                var mockID = shopConfig.ID;
                _shopUIItemInstances[i].BuyRequestEvent
                    .Subscribe(_ => BuyBtnPressEvent?.Execute(mockID))
                    .AddTo(_itemsHooksHandler);
                
                _typesDefinitionBrowser[type].Add(i);
            }

            InitFilterBtns();
        }

        public void ShowOnlyType(UniqItemsType type)
        {
            for (var i = 0; i < _shopUIItemInstances.Count; i++)
            {
                _shopUIItemInstances[i].gameObject.SetActive(_typesDefinitionBrowser[type].Contains(i)); 
            }

            _currentFilter = type;
        }


        private void UpdateScrollContentSize(int size)
        {
            var newItemsForInstanceCount = size - _shopUIItemInstances.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_shopUIItemPrefab, _container);
                newItem.transform.localScale = Vector3.one;
                _shopUIItemInstances.Add(newItem);
            }

            for (var i = _shopUIItemInstances.Count - 1; i >= _shopUIItemInstances.Count + newItemsForInstanceCount; i--)
            {
                _shopUIItemInstances[i].gameObject.SetActive(false);
            }
        }

        private void InitFilterBtns()
        {
            FilterBtnBinder btnBinder = null;
            
            foreach (var filterBtn in _filterBtns)
            {
                if (_typesDefinitionBrowser.ContainsKey(filterBtn.uniqItemsType))
                {
                    filterBtn.button.gameObject.SetActive(true);
                    if (btnBinder != null) continue;
                    
                    btnBinder = filterBtn;
                }
                else
                {
                    filterBtn.button.gameObject.SetActive(false);
                }
            }

            if (btnBinder != null)
            {
                ShowOnlyType(btnBinder.uniqItemsType);
                btnBinder.button.image.sprite = _selectedFilterBtnSprite;
            }

            _coins.text = _gameplayService.Data.GlobalCoins.ToString();
        }

        private void HandleItemRemove(string id)
        {
            var type = _gameConfigProvider.TryDefineTypeByID(id);
            _shopUIItemInstances[_shopUIItemInstancesIdsMap[id]].gameObject.SetActive(false);
            _typesDefinitionBrowser[type].Remove(_shopUIItemInstancesIdsMap[id]);
        }
    }
}