using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Customer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public Order Order { get; private set; }
        public CustomerInfo CustomerInfo { get; private set; }
        
        
        public Customer Init(CustomerVisual visual, Order order, CustomerInfo data)
        {
            _spriteRenderer.sprite = visual.Sprite;
            _spriteRenderer.color = visual.MainColor;
            Order = order;
            CustomerInfo = data;

            return this;
        }
    }
}