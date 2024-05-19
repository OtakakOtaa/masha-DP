using _CodeBase.Garden.Data;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
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
        
        
        private PlantConfig _plantConfig;
        private PlantDummy _plantDummy;

        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold);
            _plantDummy = Instantiate(_plantDummyPrefab, transform, true);
            _plantDummy.gameObject.SetActive(false);
        }

        
        public void Init(PlantConfig config)
        {
            if (IsAwoke is false) Awake();

            _plantConfig = config;
            _plantDummy.Init(config);

            _title.text = config.Name.ToLower();
            _mainImage.sprite = config.Sprite;
            _mainImage.preserveAspect = true;
        }

        public override void ProcessStartInteractivity()
        {
            _plantDummy.gameObject.SetActive(true);
        }

        public override void ProcessInteractivity()
        {
            _plantDummy.transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity()
        {
            _plantDummy.gameObject.SetActive(false);
        }

        public override InteractiveObject GetTargetCursorIObj()
        {
            return _plantDummy;
        }
    }
}