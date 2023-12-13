using System;
using UnityEngine;

namespace _CodeBase.Input.InteractiveObjsTypes
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class InteractiveObject : MonoBehaviour
    {
        private Collider2D _interactiveZone;
        
        
        public int Layer => gameObject.layer;
        
        private void Awake()
        {
            try
            {
                _interactiveZone = GetComponent<Collider2D>();
                _interactiveZone.isTrigger = true;
            }
            catch (Exception) { Debug.Log($"Interactive object: {gameObject.name} no has trigger zone"); }
        }
    }
}