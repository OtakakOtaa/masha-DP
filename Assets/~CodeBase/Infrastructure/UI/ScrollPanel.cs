using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace _CodeBase.Infrastructure.UI
{
    public sealed class ScrollPanel : MonoBehaviour
    {
        [SerializeField] private DraggableExecutableItem _closeTrigger;
        [SerializeField] private AnimationCurve _plantPanelAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Vector3 _plantPanelAnimationOffset;
        [SerializeField] private float _plantPanelAnimationDuration;
        [SerializeField] private BasketUIElement _UIItemPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private ScrollRect _scroll;
        
        
        private Tween _panelAnimationHandler;
        private Vector3 _panelStartAnimPos;
        private Vector3 _panelEndAnimPos;
        private Vector3 _panelOriginPos;
        private bool _isPanelOpening;
        private float _currentAnimProgress;

        private readonly List<BasketUIElement> _items = new();
        
        
        public ReactiveCommand OnClosed => _closeTrigger.OnExecuted;
        
        
        private void Awake()
        {
            _panelOriginPos = transform.position;
            _currentAnimProgress = 1f;
        }


        private void OnEnable()
        {
            UpdatePanelState(true);
            _panelAnimationHandler.OnComplete(() =>
            {
                _closeTrigger.enabled = true;
                _closeTrigger.SetCurrentPositionAsInit();
            });

        }

        private void OnDisable()
        {
            _closeTrigger.enabled = false;
            _isPanelOpening = false;
            transform.position = PanelHidePosition;
        }


        public void UpdateData(IUniq[] configs)
        {
            var newItemsForInstanceCount = configs.Length - _items.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_UIItemPrefab, _container.transform);
                newItem.transform.localScale = Vector3.one;
                _items.Add(newItem);
            }

            for (var i = _items.Count - 1; i >= _items.Count + newItemsForInstanceCount; i--)
            {
                _items[i].gameObject.SetActive(false);
            }

            for (var i = 0; i < configs.Length; i++)
            {
                _items[i].gameObject.SetActive(true);
                _items[i].Init(configs[i], _scroll);
            }
        }

        
        private void UpdatePanelState(bool isOpenRequired)
        {
            _panelAnimationHandler?.Kill();

            _panelEndAnimPos = isOpenRequired ? _panelOriginPos : PanelHidePosition;
            _panelStartAnimPos = transform.position;
            _isPanelOpening = isOpenRequired;
                
            _panelAnimationHandler = DOTween.To(UpdatePanelAnim, startValue: 0f, endValue: 1f, _plantPanelAnimationDuration * _currentAnimProgress);
        }

        
        private void UpdatePanelAnim(float t)
        {
            _currentAnimProgress = 1f - (transform.position - _panelEndAnimPos).magnitude / (_panelOriginPos - PanelHidePosition).magnitude;
            
            t = !_isPanelOpening ? 1 - _plantPanelAnimationCurve.Evaluate(1 - t) : _plantPanelAnimationCurve.Evaluate(t);

            transform.position = Vector3.LerpUnclamped(_panelStartAnimPos, _panelEndAnimPos, t);
        }

        private Vector3 PanelHidePosition => _panelOriginPos + _plantPanelAnimationOffset;
    }
}