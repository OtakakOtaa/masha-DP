using _CodeBase.Garden.UI;
using UnityEngine;

namespace _CodeBase.MainGameplay
{
    public sealed class GameplayUIBinder : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private GameplayHudUI _hud;
        [SerializeField] private GardenUI _gardenUI;

        
        public GardenUI GardenUI => _gardenUI;
        public GameplayHudUI HudUI => _hud;
        public Canvas MainCanvas => _mainCanvas;
    }
}