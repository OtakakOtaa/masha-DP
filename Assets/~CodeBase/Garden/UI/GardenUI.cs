using System.Collections.Generic;
using _CodeBase.Garden.Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenUI : MonoBehaviour 
    {
        [SerializeField] private Button _panelWithPlantBtn;
        [SerializeField] private PlantPanel _plantPanel;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private GameObject _plantContentContainer;
        [SerializeField] private GardenPlantUIItem _gardenPlantUIItemPrefab;
        [SerializeField] private DraggableExecutableItem _openPanelExecutableItem;
        

        
        private readonly CompositeDisposable _subscriptions = new();
        
        private readonly List<GardenPlantUIItem> _plantItems = new();
        
        public void Init()
        {
            _openPanelExecutableItem.OnExecuted.Subscribe(_ => OpenPanel()).AddTo(_subscriptions);
            _plantPanel.OnClosed.Subscribe(_ => ClosePanel()).AddTo(_subscriptions);
            
            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());
        }

        
        public void HardResetPanelToDefault()
        {
            _panelWithPlantBtn.gameObject.SetActive(true);
            _plantPanel.gameObject.SetActive(false);
            
            _openPanelExecutableItem.SetToDefault();
        }

        public void UpdatePlantsData(PlantConfig[] plantsConfigs)
        {
            var newItemsForInstanceCount = plantsConfigs.Length - _plantItems.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_gardenPlantUIItemPrefab, _plantContentContainer.transform);
                newItem.transform.localScale = Vector3.one;
                _plantItems.Add(newItem);
            }

            for (var i = _plantItems.Count - 1; i >= _plantItems.Count + newItemsForInstanceCount; i--)
            {
                _plantItems[i].gameObject.SetActive(false);
            }

            for (var i = 0; i < plantsConfigs.Length; i++)
            {
                _plantItems[i].gameObject.SetActive(true);
                _plantItems[i].Init(plantsConfigs[i], _scroll);
            }
        }

        
        private void OpenPanel()
        {
            _openPanelExecutableItem.gameObject.SetActive(false);
            _plantPanel.gameObject.SetActive(true);
        }

        private void ClosePanel()
        {
            _plantPanel.gameObject.SetActive(false);
            _openPanelExecutableItem.gameObject.SetActive(true);
            _openPanelExecutableItem.SetToDefault();
        }
    }
}