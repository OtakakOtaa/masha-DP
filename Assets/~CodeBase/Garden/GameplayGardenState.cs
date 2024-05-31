using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Garden.GardenBed;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using UnityEngine;
using VContainer;

namespace _CodeBase.Garden
{
    public sealed class GameplayGardenState : GameplaySceneState
    {
        [SerializeField] private GardenBedArea[] _gardenBedAreas;

        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;
        
        
        protected override void OnFirstEnter()
        {
            _gameplayService.UI.GardenUI.Init();
            
            foreach (var gardenBedArea in _gardenBedAreas)
            {
                gardenBedArea.Init(GardenBedArea.State.ReadyToUsingWithoutRestrictions);
            }
        }

        protected override void OnEnter()
        {
            _gameplayService.UI.GardenUI.gameObject.SetActive(true);
            _gameplayService.UI.GardenUI.HardResetPanelToDefault();
                
            var seeds = _gameplayService.Data.Seeds.Select(id => _gameConfigProvider.GetByID<PlantConfig>(id)).ToArray();
            _gameplayService.UI.GardenUI.FillData(seeds);
        }

        protected override void OnExit()
        {
        }
    }
}