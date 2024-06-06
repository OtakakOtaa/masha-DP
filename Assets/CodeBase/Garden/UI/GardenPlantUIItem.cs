using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using UnityEngine.UI;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenPlantUIItem : BasketUIElement<PlantConfig, ScrollRect>
    {
        private ScrollRect _scrollRect;
        
        protected override void OnInit(PlantConfig config, ScrollRect param)
        {
            _scrollRect = param;
            
            _title.text = config.Name.ToLower();
            _mainImage.sprite = config.Seed;
            _mainImage.preserveAspect = true;
        }

        
        
        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            base.ProcessStartInteractivity(inputAction);
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            _scrollRect.movementType = ScrollRect.MovementType.Elastic;
            base.ProcessEndInteractivity(inputAction);
        }
    }
}