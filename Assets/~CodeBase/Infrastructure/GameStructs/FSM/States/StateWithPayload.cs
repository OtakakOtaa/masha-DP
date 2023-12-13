namespace _CodeBase.Infrastructure.GameStructs.FSM.States
{
    public interface IGameStateWithPayload<in TPayload> : IGameState  
        where TPayload : struct
    {
        void Enter(TPayload payload);
    }
}