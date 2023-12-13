using _CodeBase._GameCycle.States.Game.GameplayMapChunks;
using _CodeBase.Infrastructure.GameStructs.FSM.States;

namespace _CodeBase._GameCycle.States.Game.Potion
{
    public sealed class GameplayPotionState : IGameState
    {
        private readonly PotionMapChunk _potionMap;
        public GameplayPotionState()
        {
            _potionMap = UnityEngine.Object.FindObjectOfType<PotionMapChunk>();
            _potionMap?.gameObject.SetActive(false);
        }
        
        public void Enter()
        {
        }
    }
}