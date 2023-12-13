using _CodeBase._GameCycle.States.MainMenu;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _CodeBase._GameCycle.States
{
    public sealed class BootstrapGameState : MonoBehaviour, IGameState
    {
        [Inject] private readonly GlobalStateMachine _gameStateMachine;
        [Inject] private readonly ScenesConfiguration _scenes;

        // EntryPoint //
        public void Start()
        { 
            Enter();
        }

        public async void Enter()
        {
            await SceneManager.LoadSceneAsync(_scenes.GetPath(GameScene.MainMenu));
            _gameStateMachine.Enter<MainMenuGameState>();
        }
    }
}