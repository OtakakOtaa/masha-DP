using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase._GameCycle.States.Game
{
    public sealed class GameHudBinder : MonoBehaviour
    {
        private GameplayRootState _gameplayRootState;

        public void Bind(GameplayRootState gameplayRootState)
            => _gameplayRootState = gameplayRootState;


        [Button]
        public void GoToGarden()
            => _gameplayRootState.SwitchToGarden();
        
        [Button]
        public void GoToLaboratory()
            => _gameplayRootState.SwitchToLaboratory();

        [Button]
        public void GoBack()
            => _gameplayRootState.BackTo();


    }
}