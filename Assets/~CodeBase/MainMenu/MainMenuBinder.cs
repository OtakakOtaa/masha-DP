using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.MainMenu
{
    public sealed class MainMenuBinder : MonoBehaviour
    {
        private MainMenuGameState _mainMenuGameState;

        public void SetHandler(MainMenuGameState mainMenuGameState)
        {
            _mainMenuGameState = mainMenuGameState;
        }
        
        [Button] 
        public void ForceStartGame() => 
            _mainMenuGameState.StartGame().Forget();
    }
}