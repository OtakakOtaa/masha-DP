using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.Manager;
using UnityEngine;

namespace _CodeBase.Input.InteractiveObjsTypes
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractiveObject : MonoBehaviour
    {
        private Collider _interactiveZone;
        private InputManager.InputAction[] _supportedActions;
        
        public IEnumerable<InputManager.InputAction> SupportedActions => _supportedActions;
        
        
        private void Awake()
        {
            gameObject.layer = C.IntractableLayer;
            _interactiveZone = GetComponent<Collider>();
            _interactiveZone.isTrigger = true;
        }

        protected void InitSupportedActionsList(params InputManager.InputAction[] list)
        {
            _supportedActions = list.Distinct().ToArray();
        }
        

        public abstract void ProcessInteractivity();

        public virtual void ProcessEndInteractivity() { }
    }
}