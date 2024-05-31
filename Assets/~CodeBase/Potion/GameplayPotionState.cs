using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _CodeBase.Potion
{
    public sealed class GameplayPotionState : GameplaySceneState
    {
        [SerializeField] private EssenceBottle[] _essenceBottles;
        [FormerlySerializedAs("_essenceMixerBase")] [SerializeField] private EssenceMixer _essenceMixer;
        [SerializeField] private SpriteRenderer[] _topRenderingObjects;
        [SerializeField] private PotionCauldron _potionCauldron;
        
        
        [Inject] private GameplayService _gameplayService;
        [Inject] private GameConfigProvider _gameConfigProvider;

        
        protected override void OnFirstEnter()
        {
            var sub = new CompositeDisposable();
            
            var availablePotionsForCraft = CalculateAllAvailablePotions();
            
            _gameplayService.UI.PotionUI.Init(_topRenderingObjects, _potionCauldron);
            _gameplayService.UI.PotionUI.FillRecipesData(availablePotionsForCraft);

            _gameplayService.UI.PotionUI.AcceptCreatedPotionEvent.Subscribe(HandlePotionCreation).AddTo(sub);
            _potionCauldron.AddPlantEvent.Subscribe(HandlePlantConsummation).AddTo(sub);
            
            gameObject.OnDestroyAsObservable().First().Subscribe(_ => sub.Dispose());
            
            
            InitEssenceBottles();
            
            _essenceMixer.gameObject.SetActive(false);
            if (_gameplayService.Data.UniqItems.Contains(_gameConfigProvider.MixerUniqId))
            {
                _essenceMixer.Init(availablePotionsForCraft);
                _essenceMixer.gameObject.SetActive(true);
            }
            
            _potionCauldron.Init(availablePotionsForCraft);
        }

        protected override void OnEnter()
        {
            _gameplayService.UI.PotionUI.gameObject.SetActive(true);
            _gameplayService.UI.PotionUI.HardResetPanelToDefault();
            
            var plants = _gameplayService.Data.HarvestPlants.Select(id => _gameConfigProvider.GetByID<PlantConfig>(id)).ToArray();
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

            var reachedEssencesIds = _gameplayService.Data.AccessibleEssences.ToArray();
            
            for (var i = 0; i < reachedEssencesIds.Length; i++)
            {
                var conf = _gameConfigProvider.GetByID<EssenceConfig>(reachedEssencesIds[i]);
                if (conf == null) throw new Exception($"{reachedEssencesIds[i]} can't found");
                
                var typedBottle = _essenceBottles.FirstOrDefault(e => e.EssenceID == conf.ID);
                if (typedBottle == default) throw new Exception($"{conf.ID} can't found needed bottle in scene");
                
                
                typedBottle.InitData(conf, typedBottle.MaxAvailableSipsCount);
                typedBottle.gameObject.SetActive(true);
            }
        }
        
        private ICollection<PotionConfig> CalculateAllAvailablePotions()
        {
            var allPotions = _gameConfigProvider.Potions.ToArray();
            var availablePotionsForCraft = new List<PotionConfig>();
            var accessedComponentsIDs = _gameplayService.Data.Seeds.Concat(_gameplayService.Data.AccessibleEssences);
            
            if (_gameplayService.Data.UniqItems.Contains(_gameConfigProvider.MixerUniqId))
            {
                accessedComponentsIDs = accessedComponentsIDs.Concat(new[] { _gameConfigProvider.MixerUniqId });
            }

            
            foreach (var potion in allPotions)
            {
                var canConceivablyCreated = potion.Compound.All(c => accessedComponentsIDs.Contains(c.ID));
                if (canConceivablyCreated)
                {
                    availablePotionsForCraft.Add(potion);
                }
            }

            return availablePotionsForCraft;
        }

        private void HandlePotionCreation(bool isTacked)
        {
            if (isTacked)
            {
                _gameplayService.Data.SetCraftedPotion(_potionCauldron.TargetMix);
            }
            
            _potionCauldron.ClearCurrentMix();
        }
        
        private void HandlePlantConsummation(string id)
        {
            _gameplayService.Data.TryRemovePlant(id);
            var plants = _gameplayService.Data.HarvestPlants.Select(p => _gameConfigProvider.GetByID<PlantConfig>(p)).ToArray();
            _gameplayService.UI.PotionUI.FillPlantData(plants);
        }
    }
}