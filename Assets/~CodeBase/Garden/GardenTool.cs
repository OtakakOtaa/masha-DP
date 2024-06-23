using _CodeBase.Garden.GardenBed;
using _CodeBase.Infrastructure;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _CodeBase.Garden
{
    public sealed class GardenTool : InteractiveObject
    {
        [SerializeField] private GardenBedArea.State _resolveState;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GifAnimation _gifAnimation;
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _actionSound;

        
        [Inject] private AudioService _audioService;
        
        private Vector3 _originPosition;
        private Vector3 _originScale;
        private Quaternion _originRotation;
        private int _originDrawLayerID;
        private bool _activeHoldFlag;
        
        public GardenBedArea.State Resolve => _resolveState;

        
        
        protected override void OnAwake()
        {
            _originPosition = transform.position;
            _originRotation = transform.rotation;
            _originScale = transform.localScale;
            _originDrawLayerID = _spriteRenderer.sortingLayerID;
            
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }


        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            _spriteRenderer.sortingLayerID = _inputManager.GameplayCursor.TargetSpriteLayerOrder;

            _activeHoldFlag = true;
        }
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            transform.position = _inputManager.WorldPosition;
        }

        public override async void ProcessEndInteractivity(InputManager.InputAction inputAction)
        { 
            _audioService.PlayEffect(_actionSound);
            await _gifAnimation.Play(); 
            _spriteRenderer.sortingLayerID = _originDrawLayerID;
            transform.position = _originPosition;
            transform.rotation = _originRotation;
            transform.localScale = _originScale;
            _activeHoldFlag = false;
        }

        public override InteractiveObject GetHandleTarget() => this;
        public override bool CanInteractiveNow => !_activeHoldFlag;
    }
}