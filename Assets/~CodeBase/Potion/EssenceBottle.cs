﻿using System;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace _CodeBase.Potion
{
    public sealed class EssenceBottle : InteractiveObject
    {
        [SerializeField] private EssenceBottleShader _essenceBottleShader;
        [SerializeField] private Slider _amountViewer;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private SpriteRenderer _neckOfVessel;
        [SerializeField] private float _regenDuration = 5f; 
        [SerializeField] private int _sipCount = 4; 
        
        
        [ValueDropdown("@MashaEditorUtility.GetAllEssenceID()")]
        [SerializeField] private string _essenceID;
        
        
        private string _originalLayerName;
        private Vector3 _originalPos;
        private EssenceConfig _essenceConfig;
        private float _startRegenPoint;
        private int _availableSipsCounter;
        private float _amountRat;
        private bool _needRegenFlag;


        public string EssenceID => _essenceID;
        public bool IsRegenerateNow => (Time.time - _startRegenPoint) < _regenDuration;
        public int MaxAvailableSipsCount => _sipCount;
        

        protected override void OnAwake()
        {
            _originalPos = transform.position;
            InitSupportedActionsList(InputManager.InputAction.Hold);

            var sub = GameService.GameUpdate.Subscribe(_ => UpdateRegen());
            gameObject.OnDestroyAsObservable().First().Subscribe(_ => sub?.Dispose());
        }

        public void InitData(EssenceConfig essenceConfig, int startSipsAmount)
        {
            _essenceConfig = essenceConfig;
            _availableSipsCounter = startSipsAmount;
            _essenceBottleShader.SetProgress(startSipsAmount / (float)_sipCount);
            _essenceBottleShader.SetColor(_essenceConfig.Color);
            _startRegenPoint = -_regenDuration;
            
            if (startSipsAmount == 0) _needRegenFlag = true;
        }

        
        private void UpdateRegen()
        {
            _amountViewer.gameObject.SetActive(false);
            if (!IsRegenerateNow) return;

            _amountViewer.gameObject.SetActive(true);
            _amountRat = Mathf.Clamp01((Time.time - _startRegenPoint) / _regenDuration);
            _amountViewer.value = _amountRat;
            _essenceBottleShader.SetProgress(_amountRat);
        }


        public bool TrySpendOneSip()
        {
            if (IsRegenerateNow || _needRegenFlag) return false;

            _availableSipsCounter--;
            _essenceBottleShader.SetProgress(_availableSipsCounter / (float)_sipCount);
            
            if (_availableSipsCounter > 0) return true;
            _needRegenFlag = true;
            return true;
        }
        
        public void ResetAndStartRegen()
        {
            _needRegenFlag = false;
            _startRegenPoint = Time.time;
            _availableSipsCounter = _sipCount;
        }


        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            if (_needRegenFlag)
            {
                ResetAndStartRegen();
                return;
            }
            
            if (IsRegenerateNow) return;
            
            _spriteRenderer.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID; 
            _neckOfVessel.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID;
        }

        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (IsRegenerateNow) return;
            
            transform.position = _inputManager.WorldPosition;
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            _spriteRenderer.sortingLayerName = _originalLayerName;
            _neckOfVessel.sortingLayerName = _originalLayerName;
            transform.position = _originalPos;
        }

        public override string GetTargetID()
        {
            return _essenceConfig.ID;
        }
    }
}