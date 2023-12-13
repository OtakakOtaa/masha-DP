using System;
using _CodeBase._GameCycle.States.Game.Garden;
using _CodeBase._GameCycle.States.Game.Hall;
using _CodeBase._GameCycle.States.Game.Potion;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _CodeBase._GameCycle.States.Game
{
    public sealed class GameplayRootState : IGameState
    {
        private static readonly TimeSpan GameTime = new(0, 0, minutes: 2, 0);
        
        private readonly ScenesConfiguration _scenesConfiguration;
        private readonly GlobalStateMachine _globalStateMachine;
        private GameplayGardenState _gardenState;
        private GameplayPotionState _potionState;
        private GameplayHallState _hallState;
        
        private Timer _gameTimer;
        
        public GameplayRootState(ScenesConfiguration scenesConfiguration, GlobalStateMachine stateMachine)
        {
            _globalStateMachine = stateMachine;
            _scenesConfiguration = scenesConfiguration;
        }
        
        public async void Enter()
        {
            await SceneManager.LoadSceneAsync(_scenesConfiguration.GetPath(GameScene.Hall), LoadSceneMode.Additive);
            _hallState = _globalStateMachine.GetState<GameplayHallState>();
            await SceneManager.LoadSceneAsync(_scenesConfiguration.GetPath(GameScene.Garden), LoadSceneMode.Additive);
            _gardenState = _globalStateMachine.GetState<GameplayGardenState>();
            await SceneManager.LoadSceneAsync(_scenesConfiguration.GetPath(GameScene.Laboratory), LoadSceneMode.Additive);
            _potionState = _globalStateMachine.GetState<GameplayPotionState>();
            
            InitState();
            _hallState.Enter();
        }

        public void SwitchToGarden()
            => _gardenState.Enter();

        public void SwitchToLaboratory()
            => _potionState.Enter();

        public void BackTo()
        {
        }

        private void InitState()
        {
            UnityEngine.Object.FindObjectOfType<GameHudBinder>().Bind(this);
            _gameTimer = new Timer();
            _gameTimer.Run(GameTime);
            ObserveGameEnd().Forget();
        }
        
        private async UniTaskVoid ObserveGameEnd()
        {
            await UniTask.WaitUntil(() => _gameTimer.IsTimeUp);
        }
    }
}