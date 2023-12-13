using _CodeBase._GameCycle.States.Game;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _CodeBase._GameCycle.States.MainMenu
{
    public sealed class MainMenuGameState : IGameState
    {
        private readonly ScenesConfiguration _scenesConfiguration;
        private readonly GlobalStateMachine _globalStateMachine;
        private MainMenuBinder _mainMenuBinder;
        public MainMenuGameState(ScenesConfiguration scenesConfiguration, GlobalStateMachine stateMachine)
        {
            _scenesConfiguration = scenesConfiguration;
            _globalStateMachine = stateMachine;
        }

        public void Enter()
        {
            _mainMenuBinder = UnityEngine.Object.FindObjectOfType<MainMenuBinder>();
            _mainMenuBinder.SetHandler(mainMenuGameState: this);
        }

        public async UniTaskVoid StartGame()
        {
            await SceneManager.LoadSceneAsync(_scenesConfiguration.GetPath(GameScene.Gameplay));
            _globalStateMachine.Enter<GameplayRootState>();
        }
    }
}