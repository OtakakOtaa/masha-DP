using _CodeBase.Input.InteractiveObjsTypes;
using UnityEngine;

namespace _CodeBase.Garden.Data
{
    public sealed class PlantDummy : InteractiveObject
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        
        public PlantConfig PlantConfig { get; private set; }
        public string ID { get; private set; }

        public void Init(PlantConfig config = null, string id = null)
        {
            PlantConfig = config;
            ID = id;

            if (PlantConfig)
            {
                _spriteRenderer.sprite = PlantConfig.Sprite;
            }
        }
        
        public override void ProcessInteractivity()
        {
        }

        protected override void OnAwake()
        {
        }
    }
}