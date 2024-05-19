using System.Collections.Generic;
using _CodeBase.Garden.Data;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenUI : MonoBehaviour 
    {
        [SerializeField] private Button _panelWithPlantBtn;
        [SerializeField] private GameObject _plantPanel;
        [SerializeField] private GameObject _plantContentContainer;
        [SerializeField] private GardenPlantUIItem _gardenPlantUIItemPrefab;
        
        
        [Header("Settings")] 
        [SerializeField] private AnimationCurve _plantPanelAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Vector3 _plantPanelAnimationOffset;
        [SerializeField] private float _plantPanelAnimationDuration;
        
        
        private readonly CompositeDisposable _subscriptions = new();
        
        private Tween _panelAnimationHandler;
        private Vector3 _panelStartAnimPos;
        private Vector3 _panelEndAnimPos;
        private Vector3 _panelOriginPos;
        private bool _isPanelOpening;
        private float _currentAnimProgress;

        private readonly List<GardenPlantUIItem> _plantItems = new();
        
        public void Init()
        {
            _panelWithPlantBtn.OnClickAsObservable().Subscribe(_ => UpdatePanelState(!_isPanelOpening)).AddTo(_subscriptions);

            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());

            _panelOriginPos = _plantPanel.transform.position;
            _currentAnimProgress = 1f;
        }

        
        public void HardResetPanelToDefault()
        {
            _panelWithPlantBtn.gameObject.SetActive(true);
            _isPanelOpening = false;
            _plantPanel.transform.position = PanelHidePosition;
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
                _plantItems[i].Init(plantsConfigs[i]);
            }
        } 
        
        public void UpdatePanelState(bool isOpenRequired)
        {
            _panelAnimationHandler?.Kill();

            _panelEndAnimPos = isOpenRequired ? _panelOriginPos : PanelHidePosition;
            _panelStartAnimPos = _plantPanel.transform.position;
            _isPanelOpening = isOpenRequired;
            
            _panelAnimationHandler = DOTween.To(UpdatePanelAnim, startValue: 0f, endValue: 1f, _plantPanelAnimationDuration * _currentAnimProgress);
        }

        private void UpdatePanelAnim(float t)
        {
            _currentAnimProgress = 1f - (_plantPanel.transform.position - _panelEndAnimPos).magnitude / (_panelOriginPos - PanelHidePosition).magnitude;
            
            t = !_isPanelOpening
                ? 1 - _plantPanelAnimationCurve.Evaluate(1 - t)
                : _plantPanelAnimationCurve.Evaluate(t);

            _plantPanel.transform.position = Vector3.LerpUnclamped(_panelStartAnimPos, _panelEndAnimPos, t);
        }

        private Vector3 PanelHidePosition => _panelOriginPos + _plantPanelAnimationOffset;
    }
}