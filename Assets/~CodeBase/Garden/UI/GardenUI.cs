using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenUI : MonoBehaviour 
    {
        [SerializeField] private ScrollPanel _scrollPanel;
        [SerializeField] private PulledUIItem _openPanelBtn;
        
        
        private readonly CompositeDisposable _subscriptions = new();
        
        public void Init()
        {
            _openPanelBtn.OnExecuted.Subscribe(_ => OpenPanel()).AddTo(_subscriptions);
            _scrollPanel.OnClosed.Subscribe(_ => ClosePanel()).AddTo(_subscriptions);
            
            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());
        }

        
        public void HardResetPanelToDefault()
        {
            _openPanelBtn.gameObject.SetActive(true);
            _scrollPanel.gameObject.SetActive(false);
            
            _openPanelBtn.SetToDefaultWithAnim();
        }

        public void FillData(PlantConfig[] plantConfigs)
        {
            _scrollPanel.UpdateData(plantConfigs);
        }
        
        
        private void OpenPanel()
        {
            _openPanelBtn.gameObject.SetActive(false);
            _scrollPanel.gameObject.SetActive(true);
        }

        private void ClosePanel()
        {
            _scrollPanel.gameObject.SetActive(false);
            _openPanelBtn.gameObject.SetActive(true);
            _openPanelBtn.SetToDefaultWithAnim();
        }
    }
}