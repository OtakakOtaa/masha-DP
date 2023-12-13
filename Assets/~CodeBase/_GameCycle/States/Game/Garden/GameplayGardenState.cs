using _CodeBase._GameCycle.States.Game.GameplayMapChunks;
using _CodeBase.Infrastructure.GameStructs.FSM.States;

namespace _CodeBase._GameCycle.States.Game.Garden
{
    public sealed class GameplayGardenState : IGameState
    {
        private readonly GardenMapChunk _gardenMap;
        public GameplayGardenState()
        {
            _gardenMap = UnityEngine.Object.FindObjectOfType<GardenMapChunk>();
            _gardenMap?.gameObject.SetActive(false);
        }
        
        public void Enter()
        {
        }
    }
}