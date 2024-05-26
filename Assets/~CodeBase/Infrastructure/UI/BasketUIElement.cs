using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Infrastructure.UI
{

    
    public abstract class ScrollViewItem<TData> : ScrollViewItem
    {
        public abstract void Init(TData data, ScrollRect scrollRect);
        public sealed override void Init(object data1, object data2)
        {
            Init((TData)data1, (ScrollRect)data2);
        }
    }
    
    public abstract class ScrollViewItem : InteractiveObject
    {
        public abstract void Init(object data1, object data2);
    }

    
    public abstract class BasketUIElement<TConfig, TParam> : ScrollViewItem where TConfig : IUniq
    {
        [SerializeField] protected TMP_Text _title;
        [SerializeField] protected Image _mainImage;
        [SerializeField] protected ItemDummy<TConfig> _dummyPrefab;
        
        private TConfig _config;
        private ItemDummy<TConfig> _dummy;
        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }

        
        public void Init(TConfig config, TParam param)
        {
            if (!_dummy)
            {
                _dummy = Instantiate(_dummyPrefab, GameplayService.Instance.CurrentGameSceneMap.transform, true);
                _dummy.gameObject.SetActive(false);
            }
            
            _config = config;
            _dummy.Init(config);

            OnInit(config, param);
        }

        protected abstract void OnInit(TConfig config, TParam param);

        public override void Init(object config, object param)
        {
            Init((TConfig)config, (TParam)param);
        }


        public override void ProcessStartInteractivity(InputManager.InputAction action)
        {
            _dummy.gameObject.SetActive(true);
        }

        public override void ProcessInteractivity(InputManager.InputAction action)
        {
            _dummy.transform.position = InputManager.Instance.WorldPosition;
        }

        public override void ProcessEndInteractivity(InputManager.InputAction action)
        {
            _dummy.gameObject.SetActive(false);
        }

        
        public override InteractiveObject GetHandleTarget()
        {
            return _dummy;
        }

        public override string GetTargetID()
        {
            return _config.ID;
        }
    }
}