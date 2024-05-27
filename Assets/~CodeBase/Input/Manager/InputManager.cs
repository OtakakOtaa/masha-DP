using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
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


        public static InputManager Instance { get; private set; }
        
        [Inject] public GameplayCursor GameplayCursor { get; private set; }
        public Vector3 WorldPosition { get; private set; }
        public Vector2 ScreenMousePos { get; private set; }
        public ReactiveCommand<Vector2> ClickEvent { get; private set; } = new ();
        public bool IntractableInputFlag { get; set; } = false;

        
        private void Awake()
        {
            Instance = this;
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

        public Vector2 WorldToViewPort(Vector2 worldPos)
        {
            return _camera.WorldToViewportPoint(worldPos);
        }

        public bool IsPosInViewPort(RectTransform rect, Vector2 targetViewPortPos)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            
            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = WorldToViewPort(corners[i]);
            }

            return targetViewPortPos.x > corners[0].x && targetViewPortPos.x < corners[3].x 
                                                      && targetViewPortPos.y > corners[0].y && targetViewPortPos.x < corners[1].y;
        }
        

        private void UpdateInput()
        {
            if (!IntractableInputFlag) return;

            var inHold = _playerInput.gameplay.Hold.ReadValue<float>() is 1;
            if (inHold is false && GameplayCursor.IsHoldNow)
            {
                var currentSceneName = SceneManager.GetActiveScene().name;
                if (_currentScene != currentSceneName)
                {
                    _currentScene = currentSceneName;
                    _droppedItemsKeepers = FindObjectsByType<ObjectKeeper>(FindObjectsSortMode.None);
                }

                var nearKeeper = _droppedItemsKeepers.FirstOrDefault(k => k.CanKeep(GameplayCursor.HandleItem.transform.position));
                nearKeeper?.ProcessInteractivity(InputAction.SomeItemDropped);
                GameplayCursor.ProcessedItem.ProcessEndInteractivity(InputAction.Hold);

                GameplayCursor.DetachItem();
                return;
            }


            if (inHold && !GameplayCursor.IsHoldNow)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    var uiItem = EventSystem.current.currentSelectedGameObject;
                    if (uiItem == null) return;
                    
                    if (uiItem.TryGetComponent<InteractiveObject>(out var uiInteractiveObject) && uiInteractiveObject.SupportedActions.Contains(InputAction.Hold))
                    {
                        AttachItemToCursor(uiInteractiveObject);
                    }
                }
                else
                {
                    var hasHit = Physics.Raycast(_ray, out var hit, _camera.farClipPlane, _layerMask);
                    if (hasHit && hit.collider.TryGetComponent<InteractiveObject>(out var interactiveObject) && interactiveObject.SupportedActions.Contains(InputAction.Hold))
                    {
                        AttachItemToCursor(interactiveObject);
                    }   
                }
            }

            if (GameplayCursor.IsHoldNow)
            {
                GameplayCursor.ProcessedItem.ProcessInteractivity(InputAction.Hold);
            }
        }

        private void OnDrag(UnityEngine.InputSystem.InputAction.CallbackContext inputContext)
        {
            ScreenMousePos = inputContext.ReadValue<Vector2>();

            if (_camera == null) return;
            
            _ray = new Ray(_camera.ScreenToWorldPoint(ScreenMousePos), _camera.transform.forward);
            WorldPosition = _camera.ScreenToWorldPoint(ScreenMousePos);
            WorldPosition = new Vector3(WorldPosition.x, WorldPosition.y, 0);
            
            GameplayCursor.transform.position = WorldPosition;
        }

        private void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext inputContext)
        {
            if (!IntractableInputFlag) return;

            ClickEvent.Execute(_camera.WorldToViewportPoint(WorldPosition));
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var uiItem = EventSystem.current.currentSelectedGameObject;
                
                if (uiItem != null && uiItem.TryGetComponent<InteractiveObject>(out var uiInteractiveObject) && uiInteractiveObject.SupportedActions.Contains(InputAction.Click))
                {
                    uiInteractiveObject.ProcessInteractivity(InputAction.Click);
                }
                return;
            }
            
            
            var hasHit = Physics.Raycast(_ray, out var hit, _camera.farClipPlane, _layerMask);
            if (hasHit && hit.collider.TryGetComponent<InteractiveObject>(out var interactiveObject) && interactiveObject.SupportedActions.Contains(InputAction.Click))
            {
                interactiveObject.ProcessInteractivity(InputAction.Click);
            }
        }
        
        
        private void AttachItemToCursor(InteractiveObject interactiveObject)
        {
            GameplayCursor.AttachItem(interactiveObject, interactiveObject.GetTargetID(), interactiveObject.GetHandleTarget());
            interactiveObject?.ProcessStartInteractivity(InputAction.Hold);
        }
    }
}