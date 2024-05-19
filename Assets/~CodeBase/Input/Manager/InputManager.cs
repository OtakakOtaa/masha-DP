using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _CodeBase.Input.Manager
{
    public sealed class InputManager : MonoBehaviour
    {
        public enum InputAction
        {
            Click,
            Hold,
            SomeItemDropped
        }
        
        [SerializeField] private LayerMask _layerMask;

        private Camera _camera;
        private Ray _ray;
        private PlayerInput _playerInput;
        private string _currentScene;
        private IEnumerable<ObjectKeeper> _droppedItemsKeepers;

        [Inject] public GameplayCursor GameplayCursor { get; private set; }
        public Vector3 WorldPosition { get; private set; }
        public Vector2 ScreenMousePos { get; private set; }
        public ReactiveCommand ClickEvent { get; private set; } = new ();
        public bool IntractableInputFlag { get; set; } = false;
        

        private void Awake()
        {
            var compositeDisposable = new CompositeDisposable(); 
            
            _playerInput = new PlayerInput();
            _playerInput.gameplay.DRAG.performed += OnDrag;
            _playerInput.gameplay.Click.performed += OnClick;
            _playerInput.Enable();

            GameService.GameUpdate.Subscribe(_ => UpdateInput()).AddTo(compositeDisposable);
            
            Disposable.Create(() => _playerInput.gameplay.DRAG.performed -= OnDrag).AddTo(compositeDisposable);
            Disposable.Create(() => _playerInput.gameplay.Click.performed -= OnClick).AddTo(compositeDisposable);
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

        private void UpdateInput()
        {
            if (!IntractableInputFlag) return;
            
            var inHold = _playerInput.gameplay.Hold.ReadValue<float>() is 1;
            if (inHold is false && GameplayCursor.Item != null)
            {
                GameplayCursor.Item.ProcessEndInteractivity();

                var currentSceneName = SceneManager.GetActiveScene().name;
                if (_currentScene != currentSceneName)
                {
                    _currentScene = currentSceneName;
                    _droppedItemsKeepers = FindObjectsByType<ObjectKeeper>(FindObjectsSortMode.None);
                }

                var nearKeeper = _droppedItemsKeepers.FirstOrDefault(k => k.CanKeep(GameplayCursor.Item.transform.position));
                nearKeeper?.ProcessInteractivity();

                GameplayCursor.DetachItem();
                return;
            }


            if (inHold && GameplayCursor.Item == null)
            {
                var hasHit = Physics.Raycast(_ray, out var hit, _camera.farClipPlane, _layerMask);
                if (hasHit && hit.collider.TryGetComponent<InteractiveObject>(out var interactiveObject) && interactiveObject.SupportedActions.Contains(InputAction.Hold))
                {
                    GameplayCursor.AttachItem(interactiveObject);
                }
            }

            if (GameplayCursor.Item)
            {
                GameplayCursor.Item.ProcessInteractivity();
            }
        }

        private void OnDrag(UnityEngine.InputSystem.InputAction.CallbackContext inputContext)
        {
            ScreenMousePos = inputContext.ReadValue<Vector2>();

            if (_camera == null) return;
            
            _ray = new Ray(_camera.ScreenToWorldPoint(ScreenMousePos), _camera.transform.forward);
            WorldPosition = _camera.ScreenToWorldPoint(ScreenMousePos);
            WorldPosition.Set(WorldPosition.x, WorldPosition.y, 0);
            
            GameplayCursor.transform.position = WorldPosition;
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