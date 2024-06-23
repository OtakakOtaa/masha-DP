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
        private readonly CancellationTokenSource _stateLiveTokenSource = new();

        protected bool ActiveFlag { get; private set; }

        protected event Action StateDisposeHandler;

        protected CancellationToken StateLiveToken => _stateLiveTokenSource.Token;
        public GameObject SceneMap => sceneMapObjHolder; 
        
        
        
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
            _stateLiveTokenSource?.Cancel();
        }
    }
}