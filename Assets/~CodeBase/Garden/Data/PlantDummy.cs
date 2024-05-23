using _CodeBase.Input.InteractiveObjsTypes;
using UnityEngine;

namespace _CodeBase.Garden.Data
{
    public sealed class PlantDummy : InteractiveObject
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector3 _scale = Vector3.one;
        
        
        public PlantConfig PlantConfig { get; private set; }
        public string ID { get; private set; }

        public void Init(PlantConfig config = null, string id = null)
        {
            transform.localScale = Vector3.one;
            PlantConfig = config;
            ID = id;

            if (PlantConfig)
            {
                _spriteRenderer.sprite = PlantConfig.Seed;
            }
        }
        
        
        public override void ProcessInteractivity() { }

        protected override void OnAwake() { }
    }
}