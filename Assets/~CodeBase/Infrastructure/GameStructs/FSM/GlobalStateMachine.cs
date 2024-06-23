using System;
using System.Collections.Generic;
using _CodeBase.Infrastructure.DI;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using Cysharp.Threading.Tasks;

namespace _CodeBase.Infrastructure.GameStructs.FSM
{
    public sealed class GlobalStateMachine
    {
        private readonly DiContainer _diContainer;
        private readonly HashSet<Type> _finalStateTypes = new();
        
        public IGameState CurrentGameState { get; private set; }
        public IGameState PreviousGameState { get; private set; }
        
        public GlobalStateMachine(Type[] finalStateTypes, DiContainer diContainer)
        {
            _diContainer = diContainer;

            foreach (var state in finalStateTypes)
            {
                _finalStateTypes.Add(state);
            }
        }
        
        public void Enter<TState>(bool ignoreExit = false, Action beforeEnterAction = default) where TState : class, IGameState
        {
            CheckStateForAvailability<TState>();
            if (ignoreExit is false)
            {
                TryExit();
            }
         
            PreviousGameState = CurrentGameState;
            CurrentGameState = GetState<TState>();
            beforeEnterAction?.Invoke();
            CurrentGameState.Enter();
        }

        public void Enter<TPayload, TStateWithPayload>(TPayload payload) 
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
        {
            CheckStateForAvailability<TStateWithPayload>();
            TryExit();
            PreviousGameState = CurrentGameState;
            CurrentGameState = GetPayloadState<TPayload, TStateWithPayload>(); 
            ((TStateWithPayload)CurrentGameState).Enter(payload);
        }

        private TState GetState<TState>() where TState : class, IGameState 
            => ResolveState<TState>() as TState;

        private void TryExit()
        {
            (CurrentGameState as IExitableGameState)?.Exit();
        }


        private IGameStateWithPayload<TPayload> GetPayloadState<TPayload, TStateWithPayload>() 
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
            => ResolveStateWithPayload<TPayload, TStateWithPayload>();

        private void CheckStateForAvailability<TState>() where TState : IGameState
        {
            if (_finalStateTypes.Contains(typeof(TState)) is false) throw _notAvailableStateEx;
        }
        
        private IGameState ResolveState<TState>() where TState : class, IGameState
        {
            CheckStateForAvailability<TState>();
            return _diContainer.Scope.ResolveFromContainer<TState>();
        }

        private IGameStateWithPayload<TPayload> ResolveStateWithPayload<TPayload, TStateWithPayload>()
            where TStateWithPayload : class, IGameStateWithPayload<TPayload> 
            where TPayload : struct
        {
            CheckStateForAvailability<IGameStateWithPayload<TPayload>>();
            return _diContainer.Scope.ResolveFromContainer<TStateWithPayload>();
        }
        
        private readonly Exception _notAvailableStateEx = new("operations with this state are blocked");
    }
}