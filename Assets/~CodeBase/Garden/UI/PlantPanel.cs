using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _CodeBase.Garden.UI
{
    public sealed class PlantPanel : MonoBehaviour
    {
        [SerializeField] private DraggableExecutableItem _closeTrigger;
        [SerializeField] private AnimationCurve _plantPanelAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Vector3 _plantPanelAnimationOffset;
        [SerializeField] private float _plantPanelAnimationDuration;
        
        
        private Tween _panelAnimationHandler;
        private Vector3 _panelStartAnimPos;
        private Vector3 _panelEndAnimPos;
        private Vector3 _panelOriginPos;
        private bool _isPanelOpening;
        private float _currentAnimProgress;

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