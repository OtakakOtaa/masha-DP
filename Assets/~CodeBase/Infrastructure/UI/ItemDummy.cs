using _CodeBase.Garden.Data;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using UnityEngine;

namespace _CodeBase.Infrastructure.UI
{
    public abstract class ItemDummy<TParam> : InteractiveObject where TParam : IUniq
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected  Vector3 _scale = Vector3.one;
        
        
        public PlantConfig PlantConfig { get; protected set; }
        public string ID { get; protected set; }

        
        
        public abstract void Init(TParam param);
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction) { }

        protected override void OnAwake() { }
    }
}