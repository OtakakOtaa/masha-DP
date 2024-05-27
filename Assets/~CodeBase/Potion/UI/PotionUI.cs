﻿using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;


namespace _CodeBase.Potion.UI
{
    public sealed class PotionUI : MonoBehaviour
    {
        public const string PlusSign = "+";
        public const string EqualSign = "=";

        
        [SerializeField] private PulledUIItem _openPlantsPanelBtn;
        [SerializeField] private PulledUIItem _openCraftInfoPanelBtn;
        [SerializeField] private PulledUIItem _clearBtn;

        [SerializeField] private ScrollPanel _scrollPanel;
        [SerializeField] private GameObject _craftPanel;
        [SerializeField] private RectTransform _craftPanelRec;
        [SerializeField] private Transform _container;
        [SerializeField] private PotionRecipeUIItem _potionRecipePrefab;


        [Inject] private GameConfigProvider _gameConfigProvider;
        
        
        private readonly List<PotionRecipeUIItem> _recipesInstances = new();
        public readonly Dictionary<string, UniqItemsType> compoundsMap = new();
        

        private readonly CompositeDisposable _subscriptions = new();
        
        public void Init()
        {
            _openPlantsPanelBtn.OnExecuted.Subscribe(_ => OpenPlantPanel()).AddTo(_subscriptions);
            _scrollPanel.OnClosed.Subscribe(_ => ClosePlantPanel()).AddTo(_subscriptions);
            
            _openCraftInfoPanelBtn.OnExecuted.Subscribe(_ => OpenCraftInfoPanel()).AddTo(_subscriptions);


            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());
        }

        
        public void HardResetPanelToDefault()
        {
            DisableAll();
            ShowAllBtns();
        }

        public void FillPlantData(PlantConfig[] plantConfigs)
        {
            _scrollPanel.UpdateData(plantConfigs);
        }


        public void FillRecipesData(ICollection<PotionConfig> allAvailablePotions)
        {
            UpdateCraftRecipeSize(allAvailablePotions.Count);

            var potions = allAvailablePotions.ToArray();
            
            
            for (var i = 0; i < allAvailablePotions.Count; i++)
            {
                var rowRecordSprite = _gameConfigProvider.GetCraftRowForPotionID(potions[i].ID);
                if (rowRecordSprite == null) throw new Exception($"CantFound Sprite record for {potions[i].ID}");
                
                _recipesInstances[i].gameObject.SetActive(true); 
                _recipesInstances[i].Init(rowRecordSprite); 
            }
        }

        
        private void OpenPlantPanel()
        {
            DisableAll();
            _scrollPanel.OpenPanel();
        }

        private void ClosePlantPanel()
        {
            DisableAll();
            ShowAllBtns();
        }

        private void OpenCraftInfoPanel()
        {
            DisableAll();
            _craftPanel.gameObject.SetActive(true);
            
            InputManager.Instance.ClickEvent
                .Where(t => !InputManager.Instance.IsPosInViewPort(_craftPanelRec, t))
                .First()
                .Subscribe(_ => CloseCraftInfoPanel());
        }
        
        private void CloseCraftInfoPanel()
        {
            DisableAll();
            ShowAllBtns();
        }

        private void DisableAll()
        {
            _openPlantsPanelBtn.PlayExecuteAnimation(isNeedExecuted: false);
            _openCraftInfoPanelBtn.PlayExecuteAnimation(isNeedExecuted: false);
            _clearBtn.PlayExecuteAnimation(isNeedExecuted: false);
            
            _scrollPanel.gameObject.SetActive(false);
            _craftPanel.gameObject.SetActive(false);
        }

        private void ShowAllBtns()
        {
            _openPlantsPanelBtn.gameObject.SetActive(true);
            _openCraftInfoPanelBtn.gameObject.SetActive(true);
            _clearBtn.gameObject.SetActive(true);
            
            _openPlantsPanelBtn.SetToDefaultWithAnim();
            _openCraftInfoPanelBtn.SetToDefaultWithAnim();
            _clearBtn.SetToDefaultWithAnim();
        }
        
        private void UpdateCraftRecipeSize(int requestedSize)
        {
            var newItemsForInstanceCount = requestedSize - _recipesInstances.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_potionRecipePrefab, _container.transform);
                newItem.transform.localScale = Vector3.one;
                _recipesInstances.Add(newItem);
            }

            for (var i = _recipesInstances.Count - 1; i >= _recipesInstances.Count + newItemsForInstanceCount; i--)
            {
                _recipesInstances[i].gameObject.SetActive(false);
            }
        }
        
        
        [Obsolete]
        private void FillRecipesDataAsCells(ICollection<PotionConfig> allAvailablePotions)
        {
            var recipes = new List<(Sprite sprite, Color color, string mes)>[allAvailablePotions.Count];

            for (var i = 0; i < allAvailablePotions.Count; i++)
            {
                var potion = allAvailablePotions.ElementAt(i);
                var recipe = recipes[i] = new List<(Sprite sprite, Color color, string mes)>();

                var plants = potion.Compound.Where(c => c.Type is UniqItemsType.Plant);
                var essences = potion.Compound.Where(c => c.Type is UniqItemsType.Essence).ToArray();
                var mix = essences.FirstOrDefault(e => e.ID == _gameConfigProvider.MixerUniqId);
                var neededMix = mix != default;


                foreach (var plant in plants)
                {
                    recipe.Add((_gameConfigProvider.GetByID<PlantConfig>(plant.ID).Sprite, Color.white, string.Empty));
                    recipe.Add((null, Color.white, PlusSign));
                }

                if (essences.Length == 0 && plants.Count() != 0)
                {
                    recipe.RemoveAt(recipe.Count - 1);
                }


                if (neededMix)
                {
                    var mixVisual =
                        _gameConfigProvider.GetEssenceMixVisualData(essences.Select(e => (e.ID, e.Amount)).ToList());
                    if (mixVisual == null) throw new Exception($"Cant Find Visual For Mix {potion.ID}");
                    recipe.Add((mixVisual.Sprite, Color.white, string.Empty));
                }
                else
                {
                    foreach (var essence in essences)
                    {
                        recipe.Add((_gameConfigProvider.GetByID<EssenceConfig>(essence.ID).Sprite, Color.white,
                            string.Empty));
                        recipe.Add((null, Color.white, PlusSign));
                    }

                    if (essences.Length > 0) recipe.RemoveAt(recipe.Count - 1);
                }


                recipe.Add((null, Color.white, EqualSign));
                recipe.Add((potion.StaticSprite, Color.white, string.Empty));
            }


            UpdateCraftRecipeSize(recipes.Length);

            for (var i = 0; i < allAvailablePotions.Count; i++)
            {
                _recipesInstances[i].gameObject.SetActive(true);
                _recipesInstances[i].Init(recipes[i].ToArray());
            }
        }
    }
}