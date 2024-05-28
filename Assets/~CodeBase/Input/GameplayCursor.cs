using _CodeBase.Input.InteractiveObjsTypes;
using JetBrains.Annotations;
using UnityEngine;

namespace _CodeBase.Input
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class GameplayCursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private string _cursorLayerID = "Cursor";
        [SerializeField] private float _zPos = -5;
        
        
        
        [CanBeNull] public InteractiveObject ProcessedItem { get; private set; }
        [CanBeNull] public InteractiveObject HandleItem { get; private set; }
        [CanBeNull] public string ItemID { get; private set; }
        public bool IsHoldNow => ProcessedItem != null;
        public int TargetSpriteLayerOrder => _spriteRenderer.sortingLayerID;
        public string CursorLayerID => _cursorLayerID;
        public float ZPos => _zPos;


        public void AttachItem(InteractiveObject processedItem, string id = null, InteractiveObject handleItem = null)
        {
            HandleItem = handleItem; 
            ProcessedItem = processedItem;
            ItemID = id;
        }
        
        public void DetachItem()
        {
            HandleItem = null; 
            ProcessedItem = null;
            ItemID = null;
        }
    }
}