using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionPlantUIItem : BasketUIElement<PlantConfig, ScrollRect>
    {
        [SerializeField] private TMP_Text _amountFld;


        private ScrollRect _scrollRect;
        

        protected override void OnInit(PlantConfig config, ScrollRect param)
        {
            _scrollRect = param;
            
            _title.text = config.Name.ToLower();
            _mainImage.sprite = config.Sprite;
            _mainImage.preserveAspect = true;
            _amountFld.text = $"{GameplayService.Instance.Data.GetPlantsCount(config.ID)} X";
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