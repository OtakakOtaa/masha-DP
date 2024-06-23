using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Hall
{
    public sealed class PotionDummy : ItemDummy<PotionConfig>
    {
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _activateSFX;

        
        private Vector3 _startPos;
        
        
        protected override void OnAwake()
        {
            _startPos = transform.position;
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }
        
        
        public override void Init(PotionConfig param)
        {
            transform.localScale = _scale;
            Config = param;
            ID = param.ID;
            
            _spriteRenderer.sprite = Config.Sprite;
        }


        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            transform.position = _startPos;
        }

        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            AudioService.Instance.PlayEffect(_activateSFX);
        }
    }
}