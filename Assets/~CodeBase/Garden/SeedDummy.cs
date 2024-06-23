using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Garden
{
    public sealed class SeedDummy : ItemDummy<PlantConfig>
    {
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _activateSFX;

        
        public override void Init(PlantConfig param)
        {
            transform.localScale = _scale;
            Config = param;
            ID = param.ID;
            
            _spriteRenderer.sprite = Config.Seed;
        }

        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            AudioService.Instance.PlayEffect(_activateSFX);
        }
    }
}