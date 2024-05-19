using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using UnityEngine;

namespace _CodeBase.Garden
{
    public sealed class GardenTool : InteractiveObject
    {
        [SerializeField] private GardenBedArea.State _resolveState;

        public GardenBedArea.State Resolve => _resolveState;

        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }

        
        public override void ProcessInteractivity()
        {
        }
    }
}