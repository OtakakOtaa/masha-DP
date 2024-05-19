﻿using _CodeBase.Infrastructure;
using _CodeBase.Input.Manager;
using UnityEngine;
using VContainer;

namespace _CodeBase.Input.InteractiveObjsTypes
{
    public abstract class ObjectKeeper : InteractiveObject
    {
        [SerializeReference] private ISomeArea _someInputsArea;
        [Inject] private GameplayCursor _gameplayCursor;
        
        protected new void Awake()
        {
            InitSupportedActionsList(InputManager.InputAction.SomeItemDropped);
            base.Awake();
        }

        public abstract override void ProcessInteractivity();
        
        public bool CanKeep(Vector2 point)
        {
            return _someInputsArea.CheckPlaceIntoSurface(point);
        }
    }
}