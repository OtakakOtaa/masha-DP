using System.Linq;
using _CodeBase.DATA;
using _CodeBase.Garden.Data;
using _CodeBase.Garden.GardenBed;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;

namespace _CodeBase.Garden
{
    public sealed class GameplayGardenState : GameplaySceneState
    {
        [SerializeField] private GardenBedArea[] _gardenBedAreas;
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _ambienceSound;
        
        
        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private AudioService _audioService;
        
        
        protected override void OnFirstEnter()
        {
            _gameplayService.UI.GardenUI.Init();

            for (var i = 0; i < _gardenBedAreas.Length; i++)
            {
                var gardenBedArea = _gardenBedAreas[i];
                gardenBedArea.Init(GameSettingsConfiguration.Instance.AreaSettings[i]);
            }
            
            _audioService.PlayExtraAmbience(_ambienceSound);
            gameObject.OnDestroyAsObservable().First().Subscribe(_ => _audioService.ClearExtraAmbience(_ambienceSound));
        }

        protected override void OnEnter()
        {
            _audioService.LaunchExtraAmbience(_ambienceSound);
            
            _gameplayService.UI.GardenUI.gameObject.SetActive(true);
            _gameplayService.UI.GardenUI.HardResetPanelToDefault();
                
            var seeds = _gameplayService.Data.Seeds.Select(id => _gameConfigProvider.GetByID<PlantConfig>(id)).ToArray();
            _gameplayService.UI.GardenUI.FillData(seeds);
        }

        protected override void OnExit()
        {
            _audioService.HideExtraAmbience(_ambienceSound);
        }
    }
}