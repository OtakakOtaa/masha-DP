using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using UnityEngine;

namespace _CodeBase.Garden
{
    public sealed class GardenTool : InteractiveObject
    {
        [SerializeField] private GardenBedArea.State _resolveState;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        
        private Vector3 _originPosition;
        private Vector3 _originScale;
        private Quaternion _originRotation;
        private int _originDrawLayerID;
        
        public GardenBedArea.State Resolve => _resolveState;

        
        
        protected override void OnAwake()
        {
            _originPosition = transform.position;
            _originRotation = transform.rotation;
            _originScale = transform.localScale;
            _originDrawLayerID = _spriteRenderer.sortingLayerID;
            
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }


        public override void ProcessStartInteractivity()
        {
            _spriteRenderer.sortingLayerID = _inputManager.GameplayCursor.TargetSpriteLayerOrder;
        }
        
        public override void ProcessInteractivity()
        {
            transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity()
        {
            _spriteRenderer.sortingLayerID = _originDrawLayerID;
            
            transform.position = _originPosition;
            transform.rotation = _originRotation;
            transform.localScale = _originScale;
        }

        public override InteractiveObject GetHandleTarget() => this;
    }
}