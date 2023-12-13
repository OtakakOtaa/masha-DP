using System;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.DI;
using _CodeBase.Infrastructure.GameStructs.FSM.States;

namespace _CodeBase._GameCycle
{
    public sealed class GameStatesResolver
    {
        private readonly TypesCollector _stateTypes;
        private readonly DiResolver _diResolver;
        
        public GameStatesResolver(TypesCollector stateTypes, DiResolver diResolver)
        {
            _stateTypes = stateTypes;
            _diResolver = diResolver;
        }

        public IGameState ResolveState<TState>() where TState : class, IGameState
        {
            CheckCreationStateType<TState>();
            return _diResolver.GetSceneScopeOrGlobal().ResolveFromContainer<TState>();
        }

        public IGameStateWithPayload<TPayload> ResolveStateWithPayload<TPayload, TStateWithPayload>()
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
        {
            CheckCreationStateType<IGameStateWithPayload<TPayload>>();
            return _diResolver.GetSceneScopeOrGlobal().ResolveFromContainer<TStateWithPayload>();
        }

        private void CheckCreationStateType<TState>() where TState : IGameState
        {
            if (_stateTypes.HasType<TState>() is false)
                throw _notAvailableStateEx;
        }

        private readonly Exception _notAvailableStateEx = new("operations with this state are blocked");
    }
}