using UnityEngine;

namespace _CodeBase.Infrastructure
{
    public abstract class SomeArea : MonoBehaviour
    {
        public abstract bool CheckPlaceIntoSurface(Vector2 point);
    }
}