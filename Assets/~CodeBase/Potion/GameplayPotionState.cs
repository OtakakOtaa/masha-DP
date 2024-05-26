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
        [SerializeField] private EssenceMixer _essenceMixer;
        
        
        
        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;

        protected override void OnFirstEnter()
        {
            _gameplayService.UI.PotionUI.Init();

            InitEssenceBottles();

            
            _essenceMixer.gameObject.SetActive(false);
            if (_gameplayService.Data.UniqItems.Contains(_gameConfigProvider.MixerUniqId))
            {
                _essenceMixer.Init();
                _essenceMixer.gameObject.SetActive(true);
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

        
        
        private void InitEssenceBottles()
        {
            foreach (var essenceBottle in _essenceBottles)
            {
                essenceBottle.gameObject.SetActive(false);
            }

            var reachedEssencesIds = _gameplayService.Data.AllEssences.ToArray();
            
            for (var i = 0; i < reachedEssencesIds.Length; i++)
            {
                var conf = _gameConfigProvider.GetByID<EssenceConfig>(reachedEssencesIds[i]);
                if (conf == null) throw new Exception($"{reachedEssencesIds[i]} can't found");
                
                var typedBottle = _essenceBottles.FirstOrDefault(e => e.EssenceID == conf.ID);
                if (typedBottle == default) throw new Exception($"{conf.ID} can't found needed bottle in scene");
                
                
                typedBottle.InitData(conf);
                typedBottle.gameObject.SetActive(true);
            }
        }
    }
}