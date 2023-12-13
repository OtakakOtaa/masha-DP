using System;
using System.Collections.Generic;
using _CodeBase._GameCycle;
using _CodeBase.Infrastructure.GameStructs.FSM.States;

namespace _CodeBase.Infrastructure.GameStructs.FSM
{
    public sealed class GlobalStateMachine
    {
        private readonly TypesCollector _statesType;
        private readonly GameStatesResolver _stateResolver;
        
        public IGameState CurrentGameState { get; private set; }
        public Stack<IGameState> _deepStack = new();

        public GlobalStateMachine(TypesCollector statesType, GameStatesResolver stateResolver)
        {
            _stateResolver = stateResolver;
            _statesType = statesType;
        }
        
        public void Enter<TState>() where TState : class, IGameState
        {
            CheckStateForAvailability<TState>();
            
            Exit();
            CurrentGameState = GetState<TState>();
            CurrentGameState.Enter();
        }

        public void Enter<TPayload, TStateWithPayload>(TPayload payload) 
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
        {
            CheckStateForAvailability<TStateWithPayload>();
            Exit();
            CurrentGameState = GetPayloadState<TPayload, TStateWithPayload>(); 
            ((TStateWithPayload)CurrentGameState).Enter(payload);
        }

        public TState GetState<TState>() where TState : class, IGameState 
            => _stateResolver.ResolveState<TState>() as TState;
        
        private void Exit()
            => (CurrentGameState as IExitableGameState)?.Exit();


        private IGameStateWithPayload<TPayload> GetPayloadState<TPayload, TStateWithPayload>() 
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
            => _stateResolver.ResolveStateWithPayload<TPayload, TStateWithPayload>();

        private void CheckStateForAvailability<TState>() where TState : IGameState
        {
            if (_statesType.HasType<TState>() is false) 
                throw _notAvailableStateEx;
        }

        private readonly Exception _notAvailableStateEx = new("operations with this state are blocked");
    }
}