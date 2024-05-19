using _CodeBase.Input.InteractiveObjsTypes;
using JetBrains.Annotations;
using UnityEngine;

namespace _CodeBase.Input
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class GameplayCursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        [CanBeNull] public InteractiveObject Item { get; private set; }
        [CanBeNull] public string ItemID { get; private set; }
        
        
        public bool IsHoldNow => Item != null;
        
        
        public void AttachItem(InteractiveObject item, string id = null)
        {
            Item = item;
            ItemID = id;
        }
        
        public void DetachItem()
        {
            Item = null;
        }
    }
}