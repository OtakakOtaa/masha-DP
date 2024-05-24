using System;
using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using UnityEngine;
using VContainer;

namespace _CodeBase.Potion
{
    public sealed class GameplayPotionState : GameplaySceneState
    {
        [SerializeField] private EssenceBottle[] _essenceBottles;
        
        
        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;

        protected override void OnFirstEnter()
        {
            _gameplayService.UI.PotionUI.Init();


            
            foreach (var essenceBottle in _essenceBottles)
            {
                essenceBottle.gameObject.SetActive(false);
            }

            var reachedEssencesIds = _gameplayService.Data.AllEssences.ToArray();
            var maxInitEssencesCount = Math.Min(reachedEssencesIds.Length, _essenceBottles.Length);
            
            for (var i = 0; i < maxInitEssencesCount; i++)
            {
                var conf = _gameConfigProvider.GetByID<EssenceConfig>(reachedEssencesIds[i]);
                if (conf == null) throw new Exception("reachedEssencesIds[i] can't found");
                
                _essenceBottles[i].InitData(conf);
                _essenceBottles[i].gameObject.SetActive(true);
            }
        }

        protected override void OnEnter()
        {
            _gameplayService.UI.PotionUI.gameObject.SetActive(true);
            _gameplayService.UI.PotionUI.HardResetPanelToDefault();
            
            var plants = _gameplayService.Data.AvailablePlantsStorage.Select(id => _gameConfigProvider.GetByID<PlantConfig>(id)).ToArray();
            _gameplayService.UI.PotionUI.FillPlantData(plants);
        }

        protected override void OnExit()
        {
        }
    }
}