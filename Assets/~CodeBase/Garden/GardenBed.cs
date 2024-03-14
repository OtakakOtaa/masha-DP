using System;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using UniRx;
using UnityEngine;

namespace _CodeBase.Garden
{
    [RequireComponent(typeof(Collider))]
    public sealed class GardenBed : InteractiveObject
    {
        [SerializeField] private GardenBedUI _ui;
        [SerializeField] private PlantConfig _plantConfig;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _tag;
        

        private CompositeDisposable _compositeDisposable;
        private GardenBedData _gardenBedData;
        private TimeSpan _timeSpan;
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Start()
        {
            _compositeDisposable = new CompositeDisposable();
            GameService.GameUpdate.Subscribe(_ => OnUpdate()).AddTo(_compositeDisposable);
            
            _gardenBedData = new GardenBedData();
            InitSupportedActionsList(InputManager.InputAction.Click);
            ApplyNoPlantState();
        }

        private void OnUpdate()
        {
            if (_gardenBedData.hasPlant is false) return;

            var grownTimer = (CurrentTimeUtc - _gardenBedData.PlantingTimePoint).TotalSeconds;
            var progress = grownTimer / _plantConfig.GrowTime;
            if (progress >= 0.999f) _ui.gameObject.SetActive(false);
            
            _ui.UpdateProgressBar((float)progress);
            
            var spriteIndex = 0;
            for (var i = 0; i < _plantConfig.PlatPhases.Count; i++)
            {
                if(_plantConfig.PlatPhases[i].Progress > progress) break;
                if (_plantConfig.PlatPhases[i].Progress <= progress) spriteIndex = i;
            }

            _spriteRenderer.sprite = _plantConfig.PlatPhases[spriteIndex].Sprite;
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        public override void ProcessInteractivity()
        {
            if (_gardenBedData.hasPlant && HasPlantGrown is false) return;
            
            var isHarvesting = _gardenBedData.hasPlant && HasPlantGrown;
            if (isHarvesting)
            {
                _gardenBedData.hasPlant = false;
                GameplayService.Instance.gameplayState.AddGrownPlant(_plantConfig.PlantType);
                ApplyNoPlantState();
                return;
            }
            
            _gardenBedData.PlantingTimePoint = CurrentTimeUtc;
            _gardenBedData.hasPlant = true;
            ApplyGrownPlantState();
            _spriteRenderer.sprite = _plantConfig.PlatPhases[0].Sprite;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void ApplyNoPlantState()
        {
            _ui.gameObject.SetActive(false);
            _spriteRenderer.sprite = _tag;
        }

        private void ApplyGrownPlantState()
        {
            _ui.gameObject.SetActive(true);
            _spriteRenderer.gameObject.SetActive(true);
        }
        
        private TimeSpan CurrentTimeUtc => DateTime.UtcNow.TimeOfDay;
        private bool HasPlantGrown => (CurrentTimeUtc - _gardenBedData.PlantingTimePoint).TotalSeconds >= _plantConfig.GrowTime;
    }
}