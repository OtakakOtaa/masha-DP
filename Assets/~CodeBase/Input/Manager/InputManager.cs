using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using UniRx;
using UnityEngine;

namespace _CodeBase.Input.Manager
{
    public sealed class InputManager : MonoBehaviour
    {
        public enum InputAction
        {
            Drag,
            Click,
            Hold,
        }
        
        [SerializeField] private LayerMask _layerMask;
        
        private Camera _camera;
        private Ray _ray;
        private PlayerInput _playerInput;

        public static InputManager Instance { get; private set; }

        public ReactiveCommand ClickEvent { get; private set; } = new ();
        public bool IntractableInputFlag { get; set; } = false;
        public Vector2 MousePos { get; private set; }
        
        public InteractiveObject ActiveHoldObject { get; private set; }

        public Vector3 MousePosInWorld => _camera.ScreenToWorldPoint(MousePos);
        

        private void Awake()
        {
            Instance = this;
            _playerInput = new PlayerInput();
            _playerInput.gameplay.DRAG.performed += OnDrag;
            _playerInput.gameplay.Click.performed += OnClick;
            _playerInput.Enable();
        }

        public void OnDrawGizmos()
        {
            if(_camera == null) return;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction);
        }
        
        
        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        //HOLD
        private void Update()
        {
            if (!IntractableInputFlag) return;
            
            var inHold = _playerInput.gameplay.Hold.ReadValue<float>() is 1;
            if (inHold is false && ActiveHoldObject != null)
            {
                ActiveHoldObject.ProcessEndInteractivity();
                ActiveHoldObject = null;
                return;
            }


            if (inHold && ActiveHoldObject == null)
            {
                var hasHit = Physics.Raycast(_ray, out var hit, _camera.farClipPlane, _layerMask);
                if (hasHit && hit.collider.TryGetComponent<InteractiveObject>(out var interactiveObject) && interactiveObject.SupportedActions.Contains(InputAction.Hold))
                {
                    ActiveHoldObject = interactiveObject;
                }
            }

            if (ActiveHoldObject)
            {
                ActiveHoldObject.ProcessInteractivity();
            }
        }

        private void OnDrag(UnityEngine.InputSystem.InputAction.CallbackContext inputContext)
        {
            MousePos = inputContext.ReadValue<Vector2>();
            if (_camera != null)
            {
                _ray = new Ray(_camera.ScreenToWorldPoint(MousePos), _camera.transform.forward);
            } 
        }

        private void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext inputContext)
        {
            if (!IntractableInputFlag) return;

            ClickEvent.Execute();
            
            var hasHit = Physics.Raycast(_ray, out var hit, _camera.farClipPlane, _layerMask);
            if (hasHit && hit.collider.TryGetComponent<InteractiveObject>(out var interactiveObject) && interactiveObject.SupportedActions.Contains(InputAction.Click))
            {
                interactiveObject.ProcessInteractivity();
            }
        }
    }
}