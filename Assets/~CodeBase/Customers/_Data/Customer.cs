using UnityEngine;
using Random = System.Random;

namespace _CodeBase.Customers._Data
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Customer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public Order Order { get; private set; }
        public CustomerInfo CustomerInfo { get; private set; }
        
        
        public Customer Init(CustomerVisual visual, Order order, CustomerInfo cname)
        {
            var random = new Random();
            _spriteRenderer.color = new Color(random.Next(0, 100) / 100f, random.Next(0, 100) / 100f, random.Next(0, 100) / 100f, 1);
            _spriteRenderer.sprite = visual.Sprite;
            Order = order;
            CustomerInfo = cname;

            return this;
        }
    }
}