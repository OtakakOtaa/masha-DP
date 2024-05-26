using System.Collections.Generic;
using _CodeBase.Garden.Data;
using _CodeBase.Infrastructure.UI;
using _CodeBase.Potion.Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace _CodeBase.Potion.UI
{
    public sealed class PotionUI : MonoBehaviour
    {
        [SerializeField] private ScrollPanel _scrollPanel;
        [SerializeField] private DraggableExecutableItem _openPlantsPanelBtn;

        [SerializeField] private GameObject _craftPanel;
        [SerializeField] private Transform _container;
        [SerializeField] private Sprite _plusItemSprite;
        [SerializeField] private Sprite _equalItemSprite;
        [SerializeField] private PotionRecipeUIItem _potionRecipePrefab;
        
        
        
        private readonly List<PotionRecipeUIItem> _recipes = new();
        public readonly Dictionary<string, UniqItemsType> compoundsMap = new();
        

        private readonly CompositeDisposable _subscriptions = new();
        
        public void Init()
        {
            _openPlantsPanelBtn.OnExecuted.Subscribe(_ => OpenPlantPanel()).AddTo(_subscriptions);
            _scrollPanel.OnClosed.Subscribe(_ => ClosePlantPanel()).AddTo(_subscriptions);
            
            gameObject.OnDestroyAsObservable().Subscribe(_ => _subscriptions.Dispose());
        }

        
        public void HardResetPanelToDefault()
        {
            _openPlantsPanelBtn.gameObject.SetActive(true);
            _scrollPanel.gameObject.SetActive(false);
            
            _openPlantsPanelBtn.SetToDefault();
        }

        public void FillPlantData(PlantConfig[] plantConfigs)
        {
            _scrollPanel.UpdateData(plantConfigs);
        }


        public void FillRecipesData(PotionConfig[] allAvailablePotions)
        {
            var recipes = new List<(Sprite sprite, Color color)>[allAvailablePotions.Length];

            for (var i = 0; i < allAvailablePotions.Length; i++)
            {
                var potion = allAvailablePotions[i];
                var recipe = recipes[i] = new List<(Sprite sprite, Color color)>();
                
            }


            // UpdateCraftRecipeSize()
            

            for (var i = 0; i < allAvailablePotions.Length; i++)
            {
                _recipes[i].gameObject.SetActive(true);
                // _recipes[i].Init(allAvailablePotions[i].sprite, allAvailablePotions[i].color);
            }
        }

        
        
        
        private void OpenPlantPanel()
        {
            _openPlantsPanelBtn.gameObject.SetActive(false);
            _scrollPanel.gameObject.SetActive(true);
        }

        private void ClosePlantPanel()
        {
            _scrollPanel.gameObject.SetActive(false);
            _openPlantsPanelBtn.gameObject.SetActive(true);
            _openPlantsPanelBtn.SetToDefault();
        }

        private void UpdateCraftRecipeSize(int requestedSize)
        {
            var newItemsForInstanceCount = requestedSize - _recipes.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_potionRecipePrefab, _container.transform);
                newItem.transform.localScale = Vector3.one;
                _recipes.Add(newItem);
            }

            for (var i = _recipes.Count - 1; i >= _recipes.Count + newItemsForInstanceCount; i--)
            {
                _recipes[i].gameObject.SetActive(false);
            }
        }
    }
}