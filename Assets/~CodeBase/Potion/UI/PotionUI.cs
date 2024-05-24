using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace _CodeBase.Potion.UI
{
    public sealed class PotionUI : MonoBehaviour
    {
        [SerializeField] private ScrollPanel _scrollPanel;
        [SerializeField] private DraggableExecutableItem _openPlantsPanelBtn;
        
        
        private readonly CompositeDisposable _subscriptions = new();
        
        public void Init()
        {
            _openPlantsPanelBtn.OnExecuted.Subscribe(_ => OpenPanel()).AddTo(_subscriptions);
            _scrollPanel.OnClosed.Subscribe(_ => ClosePanel()).AddTo(_subscriptions);
            
            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());
        }

        
        public void HardResetPanelToDefault()
        {
            _openPlantsPanelBtn.gameObject.SetActive(true);
            _scrollPanel.gameObject.SetActive(false);
            
            _openPlantsPanelBtn.SetToDefault();
        }

        public void FillPlantData(PlantConfig[] plantConfigs)
        {
            _scrollPanel.UpdateData(plantConfigs);
        }
        
        
        private void OpenPanel()
        {
            _openPlantsPanelBtn.gameObject.SetActive(false);
            _scrollPanel.gameObject.SetActive(true);
        }

        private void ClosePanel()
        {
            _scrollPanel.gameObject.SetActive(false);
            _openPlantsPanelBtn.gameObject.SetActive(true);
            _openPlantsPanelBtn.SetToDefault();
        }
    }
}