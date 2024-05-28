using _CodeBase.Garden.UI;
using _CodeBase.Potion.UI;
using UnityEngine;
 
namespace _CodeBase.MainGameplay
{
    public sealed class GameplayUIBinder : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private GameplayHudUI _hud;
        [SerializeField] private GardenUI _gardenUI;
        [SerializeField] private PotionUI _potionUI;
        
        
        
        public GardenUI GardenUI => _gardenUI;
        public GameplayHudUI HudUI => _hud;
        public PotionUI PotionUI => _potionUI;

        public Canvas MainCanvas => _mainCanvas;
    }
}