﻿using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using UnityEngine.UI;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionPlantUIItem : BasketUIElement<PlantConfig, ScrollRect>
    {
        private ScrollRect _scrollRect;
        
        protected override void OnInit(PlantConfig config, ScrollRect param)
        {
            _scrollRect = param;
            
            _title.text = config.Name.ToLower();
            _mainImage.sprite = config.Sprite;
            _mainImage.preserveAspect = true;
        }
        
        
        public override void ProcessStartInteractivity()
        {
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            base.ProcessStartInteractivity();
        }

        public override void ProcessEndInteractivity()
        {
            _scrollRect.movementType = ScrollRect.MovementType.Elastic;
            base.ProcessEndInteractivity();
        }
    }
}