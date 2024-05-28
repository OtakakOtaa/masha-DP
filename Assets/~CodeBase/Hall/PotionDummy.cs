using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using UnityEngine;

namespace _CodeBase.Hall
{
    public sealed class PotionDummy : ItemDummy<PotionConfig>
    {
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
            
            _spriteRenderer.sprite = Config.StaticSprite;
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
        }
    }
}