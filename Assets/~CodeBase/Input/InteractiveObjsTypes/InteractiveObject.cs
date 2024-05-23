using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.Manager;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace _CodeBase.Input.InteractiveObjsTypes
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        [Inject] protected InputManager _inputManager;

        private Collider[] _triggers;
        private InputManager.InputAction[] _supportedActions;
        protected bool IsAwoke { get; private set; }

        public IEnumerable<InputManager.InputAction> SupportedActions => _supportedActions;

        
        protected virtual void Awake()
        {
            if (IsAwoke) return;
            
            gameObject.layer = C.IntractableLayer;
            _triggers = GetComponents<Collider>();
            _triggers.ForEach(t => t.isTrigger = true);
            OnAwake();

            IsAwoke = true;
        }

        protected void InitSupportedActionsList(params InputManager.InputAction[] list)
        {
            _supportedActions = list.Distinct().ToArray();
        }
        

        public abstract void ProcessInteractivity();

        public virtual void ProcessEndInteractivity() { }
        public virtual void ProcessStartInteractivity() { }
        
        public virtual InteractiveObject GetHandleTarget() => this;

        public virtual string GetTargetID() => null;

        protected abstract void OnAwake();
    }
}