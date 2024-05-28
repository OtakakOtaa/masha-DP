using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;

namespace _CodeBase.Garden
{
    public sealed class SeedDummy : ItemDummy<PlantConfig>
    {
        public override void Init(PlantConfig param)
        {
            transform.localScale = _scale;
            Config = param;
            ID = param.ID;
            
            _spriteRenderer.sprite = Config.Seed;
        }
    }
}