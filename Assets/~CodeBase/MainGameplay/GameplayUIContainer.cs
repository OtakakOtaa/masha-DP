using _CodeBase.Garden.UI;
using _CodeBase.Hall;
using _CodeBase.Potion.UI;
using _CodeBase.Shop;
using UnityEngine;
 
namespace _CodeBase.MainGameplay
{
    public sealed class GameplayUIContainer : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private GameplayHudUI _hud;
        [SerializeField] private GardenUI _gardenUI;
        [SerializeField] private PotionUI _potionUI;
        [SerializeField] private ShopUI _shopUI;
        [SerializeField] private DayResultsUI _dayResultsUI;


        
        public DayResultsUI DayResultsUI => _dayResultsUI;
        public GardenUI GardenUI => _gardenUI;
        public GameplayHudUI HudUI => _hud;
        public PotionUI PotionUI => _potionUI;
        public ShopUI ShopUI => _shopUI;
        public Canvas MainCanvas => _mainCanvas;
    }
}