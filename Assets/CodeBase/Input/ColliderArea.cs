using UnityEngine;

namespace _CodeBase.Input
{
    [RequireComponent(typeof(Collider))]
    public sealed class ColliderArea : SomeArea
    {
        [SerializeField] private Collider _collider;
        
        public override bool CheckPlaceIntoSurface(Vector2 point)
        {
            return _collider.bounds.Contains(new Vector3(point.x, point.y, _collider.transform.position.z));
        }
    }
}