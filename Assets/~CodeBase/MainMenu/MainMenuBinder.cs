using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _CodeBase.MainMenu
{
    public sealed class MainMenuBinder : MonoBehaviour
    {
        [Inject] private AudioService _audioService;
        [SerializeField] private string _buttonClickSfx;
        
        
        private MainMenuGameState _mainMenuGameState;

        public void SetHandler(MainMenuGameState mainMenuGameState)
        {
            _mainMenuGameState = mainMenuGameState;
        }
        
        [Button] 
        public void ForceStartGame()
        {
            _audioService.PlayEffect(_buttonClickSfx);
            _mainMenuGameState.StartGame().Forget();
        }
    }
}