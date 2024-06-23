namespace _CodeBase.Infrastructure.GameStructs.FSM.States
{
    public interface IExitableGameState : IGameState
    {
        void Exit();
    }
}