using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase._GameCycle.States.MainMenu
{
    public sealed class MainMenuBinder : MonoBehaviour
    {
        private MainMenuGameState _mainMenuGameState;

        public void SetHandler(MainMenuGameState mainMenuGameState)
        {
            _mainMenuGameState = mainMenuGameState;
        }
        
        [Button] public void ForceStartGame()
        {
            _mainMenuGameState.StartGame();
        }
    }
}