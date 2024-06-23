using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using VContainer;

namespace _CodeBase.MainGameplay
{
    public class StateForReload : IGameState
    {
        [Inject] private GameService _gameService;
        [Inject] private GlobalStateMachine _globalStateMachine;
        
        public async void Enter()
        {
            await _gameService.TryLoadScene(GameScene.EmptyScene);
            await _gameService.TryLoadScene(GameScene.Gameplay);
            _globalStateMachine.Enter<GameplayService>();
        }
    }
}