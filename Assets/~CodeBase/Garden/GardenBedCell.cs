using System;
using _CodeBase.Garden.Data;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.Garden
{
    [RequireComponent(typeof(Collider))]
    public sealed class GardenBedCell : InteractiveObject
    {
        [Inject] private GameplayService _gameplayService;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _tag;


        private GardenBedData _gardenBedData;
        private PlantConfig _plantConfig;
        private float _growingTimeOffset;

        public float Progress { get; private set; }
        public bool HasPlant => _gardenBedData.hasPlant;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnAwake()
        {
            _gardenBedData = new GardenBedData();
            InitSupportedActionsList(InputManager.InputAction.Click);
            ApplyNoPlantState();
        }
        
        
        public void UpdateProgress()
        {
            var grownTimer = (CurrentTimeUtc - _gardenBedData.PlantingTimePoint).TotalSeconds;
            Progress = (float)(grownTimer / (_plantConfig.GrowTime + _growingTimeOffset));

            var spriteIndex = 0;
            for (var i = 0; i < _plantConfig.PlatPhases.Count; i++)
            {
                if (_plantConfig.PlatPhases[i].Progress > Progress) break;
                if (_plantConfig.PlatPhases[i].Progress <= Progress) spriteIndex = i;
            }

            _spriteRenderer.sprite = _plantConfig.PlatPhases[spriteIndex].Sprite;
        }

        public void ApplyNoPlantState()
        {
            _spriteRenderer.sprite = _tag;
            Progress = 0f;
            _gardenBedData.hasPlant = false;
        }

        public void ApplyGrownPlantState(PlantConfig plantConfig, float growingTimeOffset = 0f)
        {
            Progress = 0f;
            _plantConfig = plantConfig;
            _growingTimeOffset = growingTimeOffset;

            _gardenBedData.PlantingTimePoint = CurrentTimeUtc;
            _gardenBedData.hasPlant = true;
            _spriteRenderer.sprite = _plantConfig.PlatPhases[0].Sprite;
            
            _spriteRenderer.gameObject.SetActive(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (_gardenBedData.hasPlant is false || HasPlantGrown is false) return;

            _gameplayService.Data.AddPlant(_plantConfig.ID);
            ApplyNoPlantState();
        }

        private TimeSpan CurrentTimeUtc => DateTime.UtcNow.TimeOfDay;
        private bool HasPlantGrown => (CurrentTimeUtc - _gardenBedData.PlantingTimePoint).TotalSeconds >= _plantConfig.GrowTime + _growingTimeOffset;
    }
}