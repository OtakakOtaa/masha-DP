using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using UnityEngine;

namespace _CodeBase.Garden
{
    public sealed class SeedDummy : ItemDummy<PlantConfig>
    {
        public override void Init(PlantConfig param)
        {
            transform.localScale = Vector3.one;
            PlantConfig = param;
            ID = param.ID;
            
            _spriteRenderer.sprite = PlantConfig.Seed;
        }
    }
}