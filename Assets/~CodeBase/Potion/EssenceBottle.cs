using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class EssenceBottle : InteractiveObject
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private SpriteRenderer _neckOfVessel;
        
        [ValueDropdown("@MashaEditorUtility.GetAllEssenceID()")]
        [SerializeField] private string _essenceID;
        
        
        private string _originalLayerName;
        private Vector3 _originalPos;
        private EssenceConfig _essenceConfig;

        public string EssenceID => _essenceID;


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

        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            _spriteRenderer.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID; 
            _neckOfVessel.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID;
        }

        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            _spriteRenderer.sortingLayerName = _originalLayerName;
            _neckOfVessel.sortingLayerName = _originalLayerName;
            transform.position = _originalPos;
        }
    }
}