using _CodeBase._GameCycle.States.Game.GameplayMapChunks;
using _CodeBase.Customers;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using UnityEngine.SceneManagement;

namespace _CodeBase._GameCycle.States.Game.Hall
{
    public sealed class GameplayHallState : IGameState 
    {
        private readonly CustomerFetcher _customerFetcher;
        private readonly ScenesConfiguration _scenes;
        private HallMapChunk _hallMap;

        public GameplayHallState(CustomerFetcher customerFetcher, ScenesConfiguration scenes)
        {
            _scenes = scenes;
            _customerFetcher = customerFetcher;
            _hallMap = UnityEngine.Object.FindObjectOfType<HallMapChunk>();
        }
        
        public void Enter()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scenes.GetPath(GameScene.Hall)));
            _customerFetcher.PerformNextCustomer();
        }
    }
}