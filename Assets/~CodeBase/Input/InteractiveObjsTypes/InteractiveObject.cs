using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.Manager;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace _CodeBase.Input.InteractiveObjsTypes
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractiveObject : MonoBehaviour
    {
        [Inject] protected InputManager _inputManager;

        private Collider[] _triggers;
        private InputManager.InputAction[] _supportedActions;
        
        public IEnumerable<InputManager.InputAction> SupportedActions => _supportedActions;

        
        protected virtual void Awake()
        {
            gameObject.layer = C.IntractableLayer;
            _triggers = GetComponents<Collider>();
            _triggers.ForEach(t => t.isTrigger = true);
            OnAwake();
        }

        protected void InitSupportedActionsList(params InputManager.InputAction[] list)
        {
            _supportedActions = list.Distinct().ToArray();
        }
        

        public abstract void ProcessInteractivity();

        public virtual void ProcessEndInteractivity() { }

        protected abstract void OnAwake();
    }
}