using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class EssenceBottle : InteractiveObject
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;


        private string _originalLayerName;
        private Vector3 _originalPos;
        private EssenceConfig _essenceConfig;
        
        
        protected override void OnAwake()
        {
            _originalPos = transform.position;
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }


        public void InitData(EssenceConfig essenceConfig)
        {
            _essenceConfig = essenceConfig;
        }


        
        public override string GetTargetID()
        {
            return _essenceConfig.ID;
        }

        public override void ProcessStartInteractivity()
        {
            _spriteRenderer.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID; 
        }

        public override void ProcessInteractivity()
        {
            transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity()
        {
            _spriteRenderer.sortingLayerName = _originalLayerName;
            transform.position = _originalPos;
        }
    }
}