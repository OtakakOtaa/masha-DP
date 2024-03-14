using System;
using System.Threading;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using UnityEngine;

namespace _CodeBase.Infrastructure.GameStructs
{
    public abstract class GameplaySceneState : MonoBehaviour, IExitableGameState
    {
        [SerializeField] protected GameObject sceneMapObjHolder;

        private bool _firstStartFlag;

        protected bool ActiveFlag { get; private set; }

        protected readonly CancellationTokenSource stateProcess = new();
        protected event Action StateDisposeHandler;
        
        public void Enter()
        {
            ActiveFlag = true;
            if (_firstStartFlag is false)
            {
                OnFirstEnter();
                _firstStartFlag = true;
            }
            
            OnEnter();
            sceneMapObjHolder.SetActive(true);
        }
        public void Exit()
        {
            ActiveFlag = false;
            OnExit();
            sceneMapObjHolder.SetActive(false);
        }

        private void OnDestroy()
        {
            ActiveFlag = false;
            CancelAll();
        }

        protected abstract void OnEnter();

        protected virtual void OnFirstEnter() { }

        protected abstract void OnExit();

        private void CancelAll()
        {
            StateDisposeHandler?.Invoke();
            StateDisposeHandler = null;
            stateProcess?.Cancel();
        }
    }
}