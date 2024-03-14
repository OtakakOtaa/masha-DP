using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class GuideBook : InteractiveObject
    {
        [SerializeField] private GameObject _ui;


        private void Start()
        {
            InitSupportedActionsList(InputManager.InputAction.Click);

            InputManager.Instance.ClickEvent
                .Subscribe(_ =>
                {
                    if (_ui.activeSelf) _ui.SetActive(false);
                })
                .AddTo(destroyCancellationToken);
            
            _ui.SetActive(false);
        }

        public override void ProcessInteractivity()
        {
            _ui.SetActive(true);
        }
    }
}