using UnityEngine;

namespace _CodeBase.Input
{
    public abstract class SomeArea : MonoBehaviour
    {
        public abstract bool CheckPlaceIntoSurface(Vector2 point);
    }
}