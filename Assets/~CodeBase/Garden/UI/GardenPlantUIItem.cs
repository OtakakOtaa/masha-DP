using _CodeBase.Garden.Data;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenPlantUIItem : InteractiveObject
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Image _mainImage;
        [SerializeField] private PlantDummy _plantDummyPrefab;


        private ScrollRect _scrollRect;
        private PlantConfig _plantConfig;
        private PlantDummy _plantDummy;

        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }

        
        public void Init(PlantConfig config, ScrollRect scroll)
        {
            _scrollRect = scroll;
            
            if (!_plantDummy)
            {
                _plantDummy = Instantiate(_plantDummyPrefab, GameplayService.Instance.CurrentGameSceneMap.transform, true);
                _plantDummy.gameObject.SetActive(false);
            }
            
            _plantConfig = config;
            _plantDummy.Init(config);

            _title.text = config.Name.ToLower();
            _mainImage.sprite = config.Seed;
            _mainImage.preserveAspect = true;
        }

        
        public override void ProcessStartInteractivity()
        {
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            _plantDummy.gameObject.SetActive(true);
        }

        public override void ProcessInteractivity()
        {
            _plantDummy.transform.position = InputManager.Instance.WorldPosition;
        }

        public override void ProcessEndInteractivity()
        {
            _scrollRect.movementType = ScrollRect.MovementType.Elastic;
            _plantDummy.gameObject.SetActive(false);
        }

        
        public override InteractiveObject GetHandleTarget()
        {
            return _plantDummy;
        }

        public override string GetTargetID()
        {
            return _plantConfig.ID;
        }
    }
}