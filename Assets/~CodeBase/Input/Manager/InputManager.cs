using _CodeBase.Infrastructure;
using UnityEngine.InputSystem;

namespace _CodeBase.Input.Manager
{
    public sealed class InputManager
    {
        private readonly PlayerInput _playerInput;

        private readonly Signal _holdSignal = new();
        
        public ISignal HoldSignal => _holdSignal;

        public InputManager()
        {
            _playerInput = new PlayerInput();

            _playerInput.gameplay.DRAG.performed += OnHoldSignal;
        }
        
        private void OnHoldSignal(InputAction.CallbackContext inputContext)
        {
            _holdSignal.Execute();
        }
    }
}