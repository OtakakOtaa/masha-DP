using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public sealed class Customer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        
        
        public Order Order { get; private set; }
        public CustomerInfo CustomerInfo { get; private set; }
        
        public Animator Animator => _animator;

        
        public Customer Init(CustomerVisual visual, Order order, CustomerInfo cname)
        {
            _spriteRenderer.sprite = visual.Sprite;
            Order = order;
            CustomerInfo = cname;

            return this;
        }
    }
}